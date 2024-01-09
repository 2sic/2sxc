using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Code.Internal;
using ToSic.Sxc.Oqt.Server.Plumbing;
using File = System.IO.File;
using Log = ToSic.Lib.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

/// <summary>
/// Manage app api controller compilation and registration so we can invoke action latter.
/// </summary>
internal class AppApiControllerManager: IHasLog
{
    public AppApiControllerManager(ApplicationPartManager partManager, AppApiFileSystemWatcher appApiFileSystemWatcher, ILogStore logStore, LazySvc<ThisAppCodeLoader> thisAppCodeLoader, IContextResolver ctxResolver)
    {
        _partManager = partManager;
        _thisAppCodeLoader = thisAppCodeLoader;
        _ctxResolver = ctxResolver;
        _compiledAppApiControllers = appApiFileSystemWatcher.CompiledAppApiControllers;
        Log = new Log(HistoryLogName, null, "AppApiControllerManager");
        logStore.Add(HistoryLogGroup, Log);
    }
    private readonly ConcurrentDictionary<string, bool> _compiledAppApiControllers;
    private readonly ApplicationPartManager _partManager;
    private readonly LazySvc<ThisAppCodeLoader> _thisAppCodeLoader;
    private readonly IContextResolver _ctxResolver;

    public ILog Log { get; }

    protected string HistoryLogGroup { get; } = "app-api";

    protected string HistoryLogName => "Controller.Manager";

    /// <summary>
    /// Compile and register dyncode app api controller (for new or updated app api).
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public async ValueTask<bool> PrepareController(RouteValueDictionary values)
    {
        var wrapLog = Log.Fn<bool>();

        var apiFile = (string)values["apiFile"];
        var dllName = (string)values["dllName"];

        // If we have a key (that controller is compiled and registered, but not updated) controller was prepared before, so just return values.
        // Alternatively remove older version of AppApi controller (if we got updated flag from file system watcher).
        if (_compiledAppApiControllers.TryGetValue(apiFile, out var updated))
        {
            Log.A($"_compiledAppApiControllers have value: {updated} for: {apiFile}.");
            if (updated)
                RemoveController(dllName, apiFile);
            else
                return wrapLog.ReturnTrue($"ok, nothing to do, AppApi Controller is already compiled and added to ApplicationPart: {apiFile}.");
        }

        Log.A($"We need to prepare controller for: {apiFile}.");

        // Check for AppApi file
        if (!File.Exists(apiFile))
            throw new IOException($"Error, missing AppApi file {Path.GetFileName(apiFile)}.");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)

        // Check for AppApi source code
        var apiCode = await File.ReadAllTextAsync(apiFile);
        if (string.IsNullOrWhiteSpace(apiCode))
            throw new IOException($"Error, missing AppApi code in file {Path.GetFileName(apiFile)}.");

        var appId = _ctxResolver.SetAppOrGetBlock("")?.AppState?.AppId ?? Eav.Constants.AppIdEmpty;
        // Build new AppApi Controller
        Log.A($"Compile assembly: {apiFile}, {dllName}");
        var assemblyResult = new Compiler(_thisAppCodeLoader).Compile(apiFile, dllName, appId);

        // Add new key to concurrent dictionary, before registering new AppAPi controller.
        if (!_compiledAppApiControllers.TryAdd(apiFile, false))
            throw new IOException($"Error, can't register updated controller {Path.GetFileName(apiFile)} because older controller is already registered. Please try again in few moments.");

        // Register new AppApi Controller.
        AddController(dllName, assemblyResult.Assembly);

        return wrapLog.ReturnTrue($"ok, Controller is compiled and added to ApplicationParts: {apiFile}.");
    }

    private void AddController(string dllName, Assembly assembly)
    {
        Log.A($"Add ApplicationPart: {dllName}");
        _partManager.ApplicationParts.Add(new CompilationReferencesProvider(assembly));
        // Notify change
        NotifyChange();
    }

    private void RemoveController(string dllName, string apiFile)
    {
        Log.A($"In ApplicationParts, find AppApi controller: {dllName}.");
        // In edge cases the part may be already registered more than once, so we want to really clean all
        var applicationParts = _partManager.ApplicationParts
            .Where(a => a.Name.Equals($"{dllName}.dll"))
            .ToList();

        if (applicationParts.Any())
        {
            foreach (var applicationPart in applicationParts)
            {
                Log.A($"From ApplicationParts, remove AppApi controller: {dllName}.");
                _partManager.ApplicationParts.Remove(applicationPart);

                Log.A(_compiledAppApiControllers.TryRemove(apiFile, out var removeValue)
                    ? $"Value removed: {removeValue} for {apiFile}."
                    : $"Error, can't remove value for {apiFile}.");
            }
            NotifyChange();
        }
        else
            Log.A($"In ApplicationParts, can't find AppApi controller: {dllName}");
    }

    private static void NotifyChange()
    {
        // Notify change
        AppApiActionDescriptorChangeProvider.Instance.HasChanged = true;
        AppApiActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
    }
}