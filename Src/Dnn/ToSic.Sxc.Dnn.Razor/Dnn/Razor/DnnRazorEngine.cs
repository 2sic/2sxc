using Custom.Hybrid;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Compilation;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Lib.DI;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// The razor engine, which compiles / runs engine templates
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice till v16.09")]
[EngineDefinition(Name = "Razor")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
// ReSharper disable once UnusedMember.Global
public partial class DnnRazorEngine : EngineBase, IRazorEngine, IEngineDnnOldCompatibility
{
    #region Constructor / DI

    private readonly LazySvc<CodeErrorHelpService> _errorHelp;
    private readonly CodeApiServiceFactory _codeApiServiceFactory;
    private readonly LazySvc<SourceAnalyzer> _sourceAnalyzer;
    private readonly LazySvc<ThisAppLoader> _thisAppCodeLoader;
    private readonly LazySvc<IRoslynBuildManager> _roslynBuildManager;
    private readonly AssemblyResolver _assemblyResolver;

    public DnnRazorEngine(MyServices helpers, CodeApiServiceFactory codeApiServiceFactory, LazySvc<CodeErrorHelpService> errorHelp, LazySvc<SourceAnalyzer> sourceAnalyzer, LazySvc<ThisAppLoader> thisAppCodeLoader, LazySvc<IRoslynBuildManager> roslynBuildManager, AssemblyResolver assemblyResolver) : base(helpers)
    {
        ConnectServices(
            _codeApiServiceFactory = codeApiServiceFactory,
            _errorHelp = errorHelp,
            _sourceAnalyzer = sourceAnalyzer,
            _thisAppCodeLoader = thisAppCodeLoader,
            _roslynBuildManager = roslynBuildManager,
            _assemblyResolver = assemblyResolver
        );
    }

    #endregion


    [PrivateApi]
    private RazorComponentBase EntryRazorComponent
    {
        get => Log.Getter(() => _entryRazorComponent);
        set => Log.Setter(() => _entryRazorComponent = value);
    }
    private RazorComponentBase _entryRazorComponent;

    /// <inheritdoc />
    [PrivateApi]
    public override void Init(IBlock block)
    {
        var l = Log.Fn();
        base.Init(block);
        try
        {
            EntryRazorComponent = InitWebpage(TemplatePath);
        }
        // Catch web.config Error on DNNs upgraded to 7
        catch (ConfigurationErrorsException exc)
        {
            var e = new Exception("Configuration Error. Your web.config seems to be wrong in the 2sxc folder.", exc);
            //old till 2023-05-11 " Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
            // see https://web.archive.org/web/20131201093234/http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7
            throw l.Done(e);
        }
        l.Done();
    }

    [PrivateApi]
    private HttpContextBase HttpContextCurrent => 
        _httpContext.Get(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
    private readonly GetOnce<HttpContextBase> _httpContext = new();

    [PrivateApi]
    private (TextWriter writer, List<Exception> exception) Render(RazorComponentBase page, TextWriter writer, object data)
    {
        var l = Log.Fn<(TextWriter writer, List<Exception> exception)>(message: "will render into TextWriter");
        try
        {
            if (data != null && page is ISetDynamicModel setDyn)
                setDyn.SetDynamicModel(data);
        }
        catch (Exception e)
        {
            l.Ex("Problem with setting dynamic model, error will be ignored.", e);
        }

        try
        {
            page.ExecutePageHierarchy(new(HttpContextCurrent, page, data), writer, page);
        }
        catch (Exception maybeIEntityCast)
        {
            var ex = l.Ex(_errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }

        return l.Return((writer, page.SysHlp.ExceptionsOrNull));
    }

    [PrivateApi]
    protected override (string, List<Exception>) RenderEntryRazor(RenderSpecs specs) => RenderImplementation(EntryRazorComponent, specs);

    private (string, List<Exception>) RenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        var l = Log.Fn<(string, List<Exception>)>();
        var writer = new StringWriter();
        var result = Render(webpage, writer, specs.Data);
        return l.ReturnAsOk((result.writer.ToString(), result.exception));
    }

    private object CreateWebPageInstance(string templatePath)
    {
        var l = Log.Fn<object>();
        object page = null;
        Type compiledType;
        // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
        var codeFileInfo = _sourceAnalyzer.Value.TypeOfVirtualPath(templatePath);
        try
        {
            var specWithEdition = new HotBuildSpec(App.AppId, Edition);
            l.A($"prepare spec: {specWithEdition}");

            compiledType = codeFileInfo.IsHotBuildSupported() 
                ? _roslynBuildManager.Value.GetCompiledType(codeFileInfo, specWithEdition)
                : BuildManager.GetCompiledType(templatePath);
        }
        catch (Exception compileEx)
        {
            // TODO: ADD MORE compile error help
            // 1. Read file
            // 2. Try to find base type - or warn if not found
            // 3. ...
            
            l.A($"Razor Type: {codeFileInfo}");
            var ex = l.Ex(_errorHelp.Value.AddHelpForCompileProblems(compileEx, codeFileInfo));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }

        try
        {
            if (compiledType == null)
                return l.ReturnNull("type not found");

            page = Activator.CreateInstance(compiledType);
            var pageObjectValue = RuntimeHelpers.GetObjectValue(page);
            return l.ReturnAsOk(pageObjectValue);
        }
        catch (Exception createInstanceException)
        {
            var ex = l.Ex(_errorHelp.Value.AddHelpIfKnownError(createInstanceException, page));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }
    }

    private RazorComponentBase InitWebpage(string templatePath)
    {
        var l = Log.Fn<RazorComponentBase>();
        if (string.IsNullOrEmpty(templatePath)) return l.ReturnNull("null path");

        var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance(templatePath));
        // ReSharper disable once JoinNullCheckWithUsage
        if (objectValue == null)
            throw new InvalidOperationException($"The webpage found at '{templatePath}' was not created.");

        if (objectValue is not RazorComponentBase pageToInit)
            throw new InvalidOperationException($"The webpage at '{templatePath}' must derive from RazorComponentBase.");
        //Webpage = pageToInit;

        pageToInit.Context = HttpContextCurrent;
        pageToInit.VirtualPath = templatePath;
        if (pageToInit is RazorComponent rzrPage)
        {
#pragma warning disable CS0618
            rzrPage.Purpose = Purpose;
#pragma warning restore CS0618
        }

#pragma warning disable 618, CS0612
        if (pageToInit is SexyContentWebPage oldPage) oldPage.InstancePurpose = (InstancePurposes)Purpose;
#pragma warning restore 618, CS0612

        //if (pageToInit is ICanUseRoslynCompiler appCodePage) appCodePage.AttachRazorEngine(this);

        InitHelpers(pageToInit);
        return l.ReturnAsOk(pageToInit);
    }

    private void InitHelpers(RazorComponentBase webPage)
    {
        var l = Log.Fn();
        // Only generate this for the first / top EntryRazorComponent
        // All children which are then generated here should re-use that CodeApiService
        var createCodeApiService = _sharedCodeApiService == null;
        _sharedCodeApiService ??= _codeApiServiceFactory.BuildCodeRoot(webPage, Block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel9Old);

        // If we just created a new CodeApiService, we must add this razor engine to it's piggyback
        if (createCodeApiService)
            _sharedCodeApiService.GetPiggyBack(nameof(DnnRazorEngine), () => this);
        webPage.ConnectToRoot(_sharedCodeApiService);
        l.Done();
    }

    private ICodeApiService _sharedCodeApiService;

    /// <summary>
    /// Special old mechanism to always request jQuery and Rvt
    /// </summary>
    public bool OldAutoLoadJQueryAndRvt => EntryRazorComponent._CodeApiSvc._Cdf.CompatibilityLevel <= CompatibilityLevels.MaxLevelForAutoJQuery;


    internal HelperResult RenderPage(string templatePath, object data)
    {
        var page = InitWebpage(templatePath);
        var (resultString, exceptions) = RenderImplementation(page, new(){Data = data});
        return new(writer => writer.Write(resultString));
    }
}