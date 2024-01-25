using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Compilation;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Plumbing;
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
public partial class DnnRazorEngine(
    EngineBase.MyServices helpers,
    CodeApiServiceFactory codeApiServiceFactory,
    LazySvc<CodeErrorHelpService> errorHelp,
    LazySvc<SourceAnalyzer> sourceAnalyzer,
    LazySvc<IRoslynBuildManager> roslynBuildManager)
    : EngineBase(helpers, connect: [codeApiServiceFactory, errorHelp, sourceAnalyzer, roslynBuildManager]),
        IRazorEngine, IEngineDnnOldCompatibility
{
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
    private (TextWriter writer, List<Exception> exceptions) Render(RazorComponentBase page, TextWriter writer, object data)
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
            var webPageContext = new WebPageContext(HttpContextCurrent, page, data);
            page.ExecutePageHierarchy(webPageContext, writer, page);
        }
        catch (Exception maybeIEntityCast)
        {
            var ex = l.Ex(errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }

        return l.Return((writer, page.SysHlp.ExceptionsOrNull));
    }

    [PrivateApi]
    protected override (string, List<Exception>) RenderEntryRazor(RenderSpecs specs)
    {
        var (writer, exceptions) = RenderImplementation(EntryRazorComponent, specs);
        return (writer.ToString(), exceptions);
    }

    private (TextWriter writer, List<Exception> exceptions) RenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        ILogCall<(TextWriter writer, List<Exception> exceptions)> l = Log.Fn<(TextWriter, List<Exception>)>();
        var writer = new StringWriter();
        var result = Render(webpage, writer, specs.Data);
        return l.ReturnAsOk(result);
    }

    private object CreateWebPageInstance(string templatePath)
    {
        var l = Log.Fn<object>();
        object page = null;
        Type compiledType;
        // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
        var codeFileInfo = sourceAnalyzer.Value.TypeOfVirtualPath(templatePath);
        try
        {
            var specWithEdition = new HotBuildSpec(App.AppId, Edition);
            l.A($"prepare spec: {specWithEdition}");

            compiledType = codeFileInfo.IsHotBuildSupported() 
                ? roslynBuildManager.Value.GetCompiledType(codeFileInfo, specWithEdition)
                : BuildManager.GetCompiledType(templatePath);
        }
        catch (Exception compileEx)
        {
            // TODO: ADD MORE compile error help
            // 1. Read file
            // 2. Try to find base type - or warn if not found
            // 3. ...
            
            l.A($"Razor Type: {codeFileInfo}");
            var ex = l.Ex(errorHelp.Value.AddHelpForCompileProblems(compileEx, codeFileInfo));
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
            var ex = l.Ex(errorHelp.Value.AddHelpIfKnownError(createInstanceException, page));
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

        InitHelpers(pageToInit);
        return l.ReturnAsOk(pageToInit);
    }

    private void InitHelpers(RazorComponentBase webPage)
    {
        var l = Log.Fn();
        // Only generate this for the first / top EntryRazorComponent
        // All children which are then generated here should re-use that CodeApiService
        var createCodeApiService = _sharedCodeApiService == null;
        _sharedCodeApiService ??= codeApiServiceFactory.BuildCodeRoot(webPage, Block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel9Old);

        // If we just created a new CodeApiService, we must add this razor engine to it's piggyback
        if (createCodeApiService)
            _sharedCodeApiService.GetPiggyBack(nameof(DnnRazorEngine), () => this);
        webPage.ConnectToRoot(_sharedCodeApiService);
        l.Done();
    }

    private ICodeApiService _sharedCodeApiService;

    #region Helpers for Rendering Sub-Components

    internal static HelperResult RenderSubPage(RazorComponentBase page, string templatePath, object data)
    {
        var l = (page as IHasLog).Log.Fn<HelperResult>();
        // Find the RazorEngine which MUST be on the CodeApiService PiggyBack, or throw an error
        var razorEngine = page._CodeApiSvc.PiggyBack.GetOrGenerate(nameof(DnnRazorEngine), () => (DnnRazorEngine)null)
                          ?? throw l.Ex(new Exception($"Error finding {nameof(DnnRazorEngine)}. This is very unexpected."));

        // Figure out the real path, and make sure it's lower case
        // so the ID in a cache remains the same no matter how it was called
        var path = page.NormalizePath(templatePath).ToLowerInvariant();

        var subPage = razorEngine.InitWebpage(path);
        var (writer, exceptions) = razorEngine.RenderImplementation(subPage, new() { Data = data });

        // Log any exceptions which may have occurred
        if (exceptions.SafeAny())
            exceptions.ForEach(e => l.Ex(e));

        return l.ReturnAsOk(new(w => w.Write(writer)));
    }


    #endregion

    /// <summary>
    /// Special old mechanism to always request jQuery and Rvt
    /// </summary>
    public bool OldAutoLoadJQueryAndRvt => EntryRazorComponent._CodeApiSvc._Cdf.CompatibilityLevel <= CompatibilityLevels.MaxLevelForAutoJQuery;

}