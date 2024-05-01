using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Compilation;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Compile.Internal;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Razor.Internal;

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
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
// ReSharper disable once UnusedMember.Global
internal class DnnRazorCompiler(
    EngineBase.MyServices helpers,
    CodeApiServiceFactory codeApiServiceFactory,
    LazySvc<CodeErrorHelpService> errorHelp,
    LazySvc<SourceAnalyzer> sourceAnalyzer,
    LazySvc<IRoslynBuildManager> roslynBuildManager,
    LazySvc<IAppJsonService> appJson)
    : ServiceBase<EngineBase.MyServices>(helpers, "Dnn.RzComp", connect: [codeApiServiceFactory, errorHelp, sourceAnalyzer, roslynBuildManager, appJson])
{
    protected HotBuildSpec HotBuildSpecs;
    [PrivateApi] protected IBlock Block;

    internal void SetupCompiler(HotBuildSpec specs, IBlock block)
    {
        HotBuildSpecs = specs;
        Block = block;
    }


    [PrivateApi]
    private HttpContextBase HttpContextCurrent =>
        _httpContext.Get(() => HttpContext.Current.NullOrGetWith(h => new HttpContextWrapper(h)));
    private readonly GetOnce<HttpContextBase> _httpContext = new();

    [PrivateApi]
    internal (TextWriter writer, List<Exception> exceptions) Render(RazorComponentBase page, TextWriter writer, object data)
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

        return l.Return((writer, page.RzrHlp.ExceptionsOrNull));
    }


    private (TextWriter writer, List<Exception> exceptions) RenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        ILogCall<(TextWriter writer, List<Exception> exceptions)> l = Log.Fn<(TextWriter, List<Exception>)>();
        var writer = new StringWriter();
        var result = Render(webpage, writer, specs.Data);
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

            page = Activator.CreateInstance(compiledType);
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
        if (string.IsNullOrEmpty(templatePath)) return l.ReturnNull("null path");

        // Try to build, but exit if we don't use HotBuild
        var razorBuild = CreateWebPageInstance(templatePath);
        if (exitIfNoHotBuild && !razorBuild.UsesHotBuild)
            return l.Return(new(null, false));

        var objectValue = RuntimeHelpers.GetObjectValue(razorBuild.Instance);
        // ReSharper disable once JoinNullCheckWithUsage
        if (objectValue == null)
            throw new InvalidOperationException($"The webpage found at '{templatePath}' was not created.");

        if (objectValue is not RazorComponentBase pageToInit)
            throw new InvalidOperationException($"The webpage at '{templatePath}' must derive from RazorComponentBase.");

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
        var createCodeApiService = _sharedCodeApiService == null;
        _sharedCodeApiService ??= codeApiServiceFactory.BuildCodeRoot(webPage, Block, Log, compatibilityFallback: CompatibilityLevels.CompatibilityLevel9Old);

        // If we just created a new CodeApiService, we must add this razor engine to it's piggyback
        if (createCodeApiService)
            _sharedCodeApiService.GetPiggyBack(nameof(DnnRazorCompiler), () => this);
        webPage.ConnectToRoot(_sharedCodeApiService);
        l.Done();
    }

    private ICodeApiService _sharedCodeApiService;

    #region Helpers for Rendering Sub-Components

    internal static RazorBuildTempResult<HelperResult> RenderSubPage(RazorComponentBase parent, string templatePath, object data)
    {
        var l = (parent as IHasLog).Log.Fn<RazorBuildTempResult<HelperResult>>();
        // Find the RazorEngine which MUST be on the CodeApiService PiggyBack, or throw an error
        var razorEngine = parent._CodeApiSvc.PiggyBack.GetOrGenerate(nameof(DnnRazorCompiler), () => (DnnRazorCompiler)null)
                          ?? throw l.Ex(new Exception($"Error finding {nameof(DnnRazorCompiler)}. This is very unexpected."));

        // Figure out the real path, and make sure it's lower case
        // so the ID in a cache remains the same no matter how it was called
        var path = parent.NormalizePath(templatePath).ToLowerInvariant();

        var subPage = razorEngine.InitWebpage(path, true);

        // Exit if we don't use HotBuild, because then we must revert back to classic render
        // Reason is that otherwise the PageData property - used on very old classes - would not be populated
        // Doing this from our compiler is super-hard, because it would use a lot of internal Microsoft APIs
        if (!subPage.UsesHotBuild)
            return l.Return(new(null, false), "exit, not HotBuild");

        var (writer, exceptions) = razorEngine.RenderImplementation(subPage.Instance, new() { Data = data });

        // Log any exceptions which may have occurred
        if (exceptions.SafeAny())
            exceptions.ForEach(e => l.Ex(e));

        return l.ReturnAsOk(new(new(w => w.Write(writer)), true));
    }


    #endregion

}