using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Compilation;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Compile.Sys;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Caching.PiggyBack;
using ToSic.Sys.Exceptions;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// The Razor compiler for Razor templates.
/// It's used in ca. 3 scenarios:
/// 
/// 1. Render the main razor template of a module - in this case there is no `data`
///    - it may use Roslyn or the built-in compiler
/// 2. It's then attached to the CodeApiService as a piggyback, so it can be used to render sub-components
/// 3. Render sub-components which need Roslyn
///    but exit early if Roslyn isn't necessary because of problems
///    with `data` being in `PageData` of the RazorComponent. 
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice till v16.09")]
[EngineDefinition(Name = "Razor")]
[ShowApiWhenReleased(ShowApiMode.Never)]
// ReSharper disable once UnusedMember.Global
internal class DnnRazorCompiler(
    IExecutionContextFactory exCtxFactory,
    LazySvc<CodeErrorHelpService> errorHelp,
    LazySvc<SourceAnalyzer> sourceAnalyzer,
    LazySvc<IRoslynBuildManager> roslynBuildManager,
    LazySvc<IAppJsonConfigurationService> appJson)
    : ServiceBase("Dnn.RzComp", connect: [exCtxFactory, errorHelp, sourceAnalyzer, roslynBuildManager, appJson])
{
    protected HotBuildSpec HotBuildSpecs;
    [PrivateApi] protected IBlock Block;

    internal void SetupCompiler(EngineSpecs engineSpecs)
    {
        HotBuildSpecs = engineSpecs.ToHotBuildSpec();
        Block = engineSpecs.Block;
    }


    [PrivateApi]
    private HttpContextBase HttpContextCurrent =>
        _httpContext.Get(() => HttpContext.Current.NullOrGetWith(h => new HttpContextWrapper(h)));
    private readonly GetOnce<HttpContextBase> _httpContext = new();

    [PrivateApi]
    internal (TextWriter writer, List<Exception> exceptions) Render(RazorComponentBase page, TextWriter writer, RenderSpecs renderSpecs)
    {
        var l = Log.Fn<(TextWriter writer, List<Exception> exception)>(message: "will render into TextWriter");
        try
        {
            if (page is ISetDynamicModel setDyn)
                setDyn.SetDynamicModel(renderSpecs);
        }
        catch (Exception e)
        {
            l.Ex("Problem with setting dynamic model, error will be ignored.", e);
        }

        try
        {
            var webPageContext = new WebPageContext(HttpContextCurrent, page, renderSpecs.Data);
            page.ExecutePageHierarchy(webPageContext, writer, page);
        }
        catch (Exception maybeIEntityCast)
        {
            var ex = l.Ex(errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }

        return l.Return((writer, page.RzrHlp.ExceptionsOrNull));
    }


    private (TextWriter writer, List<Exception> exceptions) RenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        ILogCall<(TextWriter writer, List<Exception> exceptions)> l = Log.Fn<(TextWriter, List<Exception>)>();
        var writer = new StringWriter();
        var result = Render(webpage, writer, specs);
        return l.ReturnAsOk(result);
    }

    private RazorBuildTempResult<object> CreateWebPageInstance(string templatePath)
    {
        var l = Log.Fn<RazorBuildTempResult<object>>(templatePath);
        object page = null;

        Type compiledType;

        // TODO: @STV SHOULD OPTIMIZE so the file doesn't need to read multiple times
        var codeFileInfo = sourceAnalyzer.Value.TypeOfVirtualPath(templatePath);
        
        var useHotBuild = appJson.Value.DnnCompilerAlwaysUseRoslyn(HotBuildSpecs.AppId) || codeFileInfo.IsHotBuildSupported();
        l.A($"{nameof(HotBuildSpecs)} prepare spec: {HotBuildSpecs}; {nameof(useHotBuild)}: {useHotBuild}");
        try
        {
            compiledType = useHotBuild
                ? roslynBuildManager.Value.GetCompiledType(codeFileInfo, HotBuildSpecs)
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

            page = TypeFactory.CreateInstance(compiledType);
            var pageObjectValue = RuntimeHelpers.GetObjectValue(page);  // seems to do unboxing, why???
            return l.ReturnAsOk(new(pageObjectValue, useHotBuild));
        }
        catch (Exception createInstanceException)
        {
            var ex = l.Ex(errorHelp.Value.AddHelpIfKnownError(createInstanceException, page));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }
    }

    public RazorBuildTempResult<RazorComponentBase> InitWebpage(string templatePath, bool exitIfNoHotBuild)
    {
        var l = Log.Fn<RazorBuildTempResult<RazorComponentBase>>();
        if (string.IsNullOrEmpty(templatePath))
            return l.ReturnNull("null path");

        // Try to build, but exit if we don't use HotBuild
        var razorBuild = CreateWebPageInstance(templatePath);
        if (exitIfNoHotBuild && !razorBuild.UsesHotBuild)
            return l.Return(new(null, false));

        var objectValue = RuntimeHelpers.GetObjectValue(razorBuild.Instance);
        // ReSharper disable once JoinNullCheckWithUsage
        if (objectValue == null)
            throw new InvalidOperationException($"The webpage found at '{templatePath}' was not created.");

        if (objectValue is not RazorComponentBase pageToInit)
            throw new ExceptionWithHelp(HelpDbRazor.AutoInheritsMissingAfterV20,
                new InvalidOperationException($"The webpage at '{templatePath}' must derive from RazorComponentBase."));

        pageToInit.Context = HttpContextCurrent;
        pageToInit.VirtualPath = templatePath;
        
        InitHelpers(pageToInit);
        return l.ReturnAsOk(new(pageToInit, razorBuild.UsesHotBuild));
    }

    private void InitHelpers(RazorComponentBase webPage)
    {
        var l = Log.Fn();
        // Only generate this for the first / top EntryRazorComponent
        // All children which are then generated here should re-use that CodeApiService
        if (_sharedCodeApiService == null)
        {
            _sharedCodeApiService = exCtxFactory
                .New(webPage, Block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel9Old);

            // Since we just created a new CodeApiService, we must add this razor engine to it's piggyback
            _sharedCodeApiService.GetPiggyBack(nameof(DnnRazorCompiler), () => this);
        }

        webPage.ConnectToRoot(_sharedCodeApiService);
        l.Done();
    }

    /// <summary>
    /// Reused CodeApiService for all Razor pages.
    /// </summary>
    private IExecutionContext _sharedCodeApiService;

    #region Helpers for Rendering Sub-Components

    internal record PrepToExecute(
        string BestPath,
        RazorBuildTempResult<RazorComponentBase> SubPage,
        DnnRazorCompiler Compiler);

    internal static PrepToExecute PrepareForRoslyn(RazorComponentBase parent, string templatePath, object data)
    {
        var l = (parent as IHasLog).Log.Fn<PrepToExecute>();

        // Find the RazorEngine which MUST be on the CodeApiService PiggyBack, or throw an error
        var razorCompiler = parent.ExCtx.PiggyBack.GetOrGenerate(nameof(DnnRazorCompiler), DnnRazorCompiler () => null)
                            ?? throw l.Ex(new Exception($"Error finding {nameof(DnnRazorCompiler)}. This is very unexpected."));

        var subPage = razorCompiler.InitWebpage(templatePath, true);

        return l.Return(new(templatePath, subPage, razorCompiler));

    }

    internal static RazorBuildTempResult<HelperResult> ExecuteWithRoslyn(PrepToExecute preparations, RazorComponentBase parent, RenderSpecs renderSpecs)
    {
        var l = (parent as IHasLog).Log.Fn<RazorBuildTempResult<HelperResult>>();

        var (writer, exceptions) = preparations.Compiler.RenderImplementation(preparations.SubPage.Instance, renderSpecs);

        // Log any exceptions which may have occurred
        if (exceptions.SafeAny())
            exceptions.ForEach(e => l.Ex(e));

        return l.ReturnAsOk(new(new(w => w.Write(writer)), true));
    }
    internal static RazorBuildTempResult<HelperResult> RenderPartialWithRoslyn(RazorComponentBase parent, string templatePath, object data, RenderSpecs renderSpecs)
    {
        var l = (parent as IHasLog).Log.Fn<RazorBuildTempResult<HelperResult>>();

        // Find the RazorEngine which MUST be on the CodeApiService PiggyBack, or throw an error
        var razorCompiler = parent.ExCtx.PiggyBack.GetOrGenerate(nameof(DnnRazorCompiler), DnnRazorCompiler () => null)
                          ?? throw l.Ex(new Exception($"Error finding {nameof(DnnRazorCompiler)}. This is very unexpected."));

        // Figure out the real path, and make sure it's lower case
        // so the ID in a cache remains the same no matter how it was called
        var path = parent.NormalizePath(templatePath).ToLowerInvariant();

        var subPage = razorCompiler.InitWebpage(path, true);

        // Exit if we don't use HotBuild, because then we must revert back to classic render
        // Reason is that otherwise the PageData property - used on very old classes - would not be populated
        // Doing this from our compiler is super-hard, because it would use a lot of internal Microsoft APIs
        if (!subPage.UsesHotBuild)
            return l.Return(new(null, false), "exit, not HotBuild");

        var (writer, exceptions) = razorCompiler.RenderImplementation(subPage.Instance, renderSpecs);

        // Log any exceptions which may have occurred
        if (exceptions.SafeAny())
            exceptions.ForEach(e => l.Ex(e));

        return l.ReturnAsOk(new(new(w => w.Write(writer)), true));
    }


    #endregion

}