using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Code.Internal;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Polymorphism.Internal;
using File = System.IO.File;
using Log = ToSic.Lib.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

/// <summary>
/// Manage app api controller compilation and registration so we can invoke action latter.
/// </summary>
internal class AppApiControllerManager : IHasLog
{
    public AppApiControllerManager(ApplicationPartManager partManager, ILogStore logStore, Generator<Compiler> compiler, IWebApiContextBuilder webApiContextBuilder, PolymorphConfigReader polymorphism,
        AppCodeLoader appCodeLoader, AppApiFileSystemWatcher appApiFileSystemWatcher)
    {
        _partManager = partManager;
        _compiler = compiler;
        _webApiContextBuilder = webApiContextBuilder;
        _polymorphism = polymorphism;
        _appCodeLoader = appCodeLoader;
        _appApiFileSystemWatcher = appApiFileSystemWatcher;
        Log = new Log(HistoryLogName, null, "AppApiControllerManager");
        logStore.Add(HistoryLogGroup, Log);
    }
    private readonly ApplicationPartManager _partManager;
    private readonly Generator<Compiler> _compiler;
    private readonly IWebApiContextBuilder _webApiContextBuilder;
    private readonly PolymorphConfigReader _polymorphism;
    private readonly AppCodeLoader _appCodeLoader;
    private readonly AppApiFileSystemWatcher _appApiFileSystemWatcher; // keep it here, because we need one instance in App

    public ILog Log { get; }

    protected string HistoryLogGroup { get; } = "app-api";

    protected string HistoryLogName => "Controller.Manager";

    private static readonly object LockObject = new();

    /// <summary>
    /// Compile and register dyncode app api controller (for new or updated app api).
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public async ValueTask<bool> PrepareController(RouteValueDictionary values)
    {
        var l = Log.Fn<bool>();

        var apiFile = (string)values["apiFile"];
        var dllName = (string)values["dllName"];
        var appFolder = (string)values["appFolder"];
        var controllerTypeName = (string)values["controllerTypeName"];
        l.A($"{nameof(apiFile)}:'{apiFile}'; {nameof(dllName)}:'{dllName}'; {nameof(appFolder)}:'{appFolder}'; {nameof(controllerTypeName)}:'{controllerTypeName}'");

        // If we have a key (that controller is compiled and registered, but not updated) controller was prepared before, so just return values.
        // Alternatively remove older version of AppApi controller (if we got updated flag from file system watcher).
        if (AppApiFileSystemWatcher.CompiledAppApiControllers.TryGetValue(apiFile, out var appApiCacheItem))
        {
            if (!appApiCacheItem.FlagForRemove)
                return l.ReturnTrue($"ok, nothing to do, AppApi Controller is already compiled and added to ApplicationPart: {apiFile}.");

            lock (LockObject) // Only one thread can enter this block at a time
            {
                // Remove older version of AppApi controller
                l.A($"remove old version of controller for: {apiFile}.");

                l.A(AppApiFileSystemWatcher.CompiledAppApiControllers.TryRemove(apiFile, out _)
                    ? $"AppApi controller cache item removed for {apiFile}."
                    : $"Error, can't remove AppApi controller cache item for {apiFile}.");

                var dllNameToRemove = (appApiCacheItem.IsAppCode ? appApiCacheItem.DllName : dllName);
                l.A($"RemoveController: {dllNameToRemove} (ApplicationParts removed: {RemoveController(apiFile, dllNameToRemove)})");
            }
        }

        l.A($"We need to prepare controller for: {apiFile}.");

        lock (LockObject) // Only one thread can enter this block at a time
        {
            // 1. check AppCode
            l.A("Search for AppApi controller in AppCode");
            var spec = BuildHotBuildSpec(appFolder);
            var (result, _) = _appCodeLoader.GetAppCode(spec);
            if (result?.Assembly != null)
            {
                l.A($"search in AppCode:{spec} for controller Type by its name {nameof(controllerTypeName)}:'{controllerTypeName}'");
                var type = result.Assembly.FindControllerTypeByName(controllerTypeName);
                if (type != null)
                {
                    l.A($"Controller Type found: {type.Name} in AppCode: {result.Assembly.GetName().Name}");


                    if (AppApiFileSystemWatcher.CompiledAppApiControllers.TryAdd(apiFile, new()
                    {
                        FlagForRemove = false,
                        IsAppCode = true,
                        AppCodePath = GetAppCodePathFromWatcherFolders(result.WatcherFolders),
                        DllName = result.Assembly.GetName().Name
                    }))
                    {
                        l.A($"{nameof(AppApiFileSystemWatcher.CompiledAppApiControllers)} AppApi controller cache item added for '{apiFile}'."); // Add new key to concurrent dictionary, before registering new AppAPi controller.
                                                                                                                                                 // Register new AppApi Controller.
                        l.A($"ApplicationPart from AppCode added: {AddController(result.Assembly)}");
                    }
                    return l.ReturnTrue("Api controller from AppCode");
                }
            }

            // 2. Check for AppApi file
            l.A($"Search for AppApi controller file:{apiFile}.");
            if (!File.Exists(apiFile))
                throw new IOException($"Error, missing AppApi file {Path.GetFileName(apiFile)}.");

            // note: this may look like something you could optimize/cache the result, but that's a bad idea
            // because when the file changes, the type-object will be different, so please don't optimize :)

            // Check for AppApi source code
            var apiCode = File.ReadAllText(apiFile);
            if (string.IsNullOrWhiteSpace(apiCode))
                throw new IOException($"Error, missing AppApi code in file '{apiFile}'.");

            // Build new AppApi Controller
            l.A($"Compile assembly: {apiFile}; {nameof(dllName)}: '{dllName}'; {spec}");
            var assemblyResult = _compiler.New().Compile(apiFile, dllName, spec);

            // Add new key to concurrent dictionary, before registering new AppAPi controller.
            if (!AppApiFileSystemWatcher.CompiledAppApiControllers.TryAdd(apiFile, new()))
                throw new IOException($"Error, can't register updated controller '{controllerTypeName}' because older controller is already registered. Please try again in few moments.");

            // Register new AppApi Controller.
            l.A($"ApplicationPart from file added: {AddController(assemblyResult.Assembly, dllName)}");
        }

        return l.ReturnTrue($"ok, Controller is compiled and added to ApplicationParts: {apiFile}.");
    }


    /// <summary>
    /// Build HotBuildSpec for AppApi controller compilation.
    /// </summary>
    /// <param name="appFolder"></param>
    /// <returns></returns>
    private HotBuildSpec BuildHotBuildSpec(string appFolder)
    {
        var l = Log.Fn<HotBuildSpec>($"{appFolder}:'{appFolder}'", timer: true);

        // Prepare / Get App State, while possibly also initializing the App...
        var ctxResolver = _webApiContextBuilder.PrepareContextResolverForApiRequest();
        var appState = ctxResolver.SetAppOrGetBlock(appFolder)?.AppReader;

        // Figure out the current edition
        var edition = FigureEdition(ctxResolver).TrimLastSlash();

        var spec = new HotBuildSpec(appState?.AppId ?? Eav.Constants.AppIdEmpty, edition: edition, appState?.Specs.Name);

        return l.ReturnAsOk(spec);
    }

    /// <summary>
    /// Figure out the current edition for HotBuildSpec.
    /// </summary>
    /// <returns></returns>
    private string FigureEdition(ISxcContextResolver ctxResolver)
    {
        var l = Log.Fn<string>(timer: true);

        var block = ctxResolver.BlockOrNull();
        var edition = block.NullOrGetWith(_polymorphism.UseViewEditionOrGet);

        return l.Return(edition);
    }

    private string GetAppCodePathFromWatcherFolders(IDictionary<string, bool> watcherFolders)
        => watcherFolders.FirstOrDefault(x => x.Key.EndsWith(Constants.AppCode)).Key;

    private bool AddController(Assembly assembly, string dllName = null)
    {
        var l = Log.Fn<bool>($"{nameof(dllName)}: '{dllName}'", timer: true);

        dllName ??= assembly.GetName().Name;
        l.A($"TryAdd ApplicationPart:'{dllName}'.");

        if (_partManager.ApplicationParts.ToList() // ToList() prevents exception 'Collection was modified; enumeration operation may not execute'
            .Any(a => a.Name.Equals($"{Path.GetFileNameWithoutExtension(dllName)}.dll")))
            return l.ReturnFalse($"OK, can't add ApplicationPart:'{dllName}' because it was already added before.");

        // Add ApplicationPart
        _partManager.ApplicationParts.Add(new CompilationReferencesProvider(assembly));

        // Notify change
        NotifyChange();

        return l.ReturnTrue($"OK, applicationPart:'{dllName}' added.");
    }

    private bool RemoveController(string apiFile, string dllName)
    {
        var l = Log.Fn<bool>($"{nameof(apiFile)}: '{apiFile}'; {nameof(dllName)}: '{dllName}'", timer: true);

        l.A($"In ApplicationParts, find AppApi controller: '{dllName}'.");
        // In edge cases the part may be already registered more than once, so we want to clean all
        var applicationParts = _partManager.ApplicationParts
            .Where(a => a.Name.Equals($"{Path.GetFileNameWithoutExtension(dllName)}.dll"))
            .ToList();

        if (!applicationParts.Any())
            return l.ReturnFalse($"In ApplicationParts, can't find AppApi controller to remove: {dllName}");

        var countRemoved = 0;
        foreach (var applicationPart in applicationParts)
        {
            if (!_partManager.ApplicationParts.Remove(applicationPart)) continue;

            l.A($"From ApplicationParts, remove AppApi controller: {dllName}.");
            countRemoved++;
        }
        if (countRemoved > 0) NotifyChange();

        return l.Return(countRemoved > 0, $"ApplicationParts removed: {countRemoved}");
    }

    private void NotifyChange()
    {
        // Notify change
        Log.A("Notify change");
        AppApiActionDescriptorChangeProvider.Instance.HasChanged = true;
        AppApiActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
    }
}