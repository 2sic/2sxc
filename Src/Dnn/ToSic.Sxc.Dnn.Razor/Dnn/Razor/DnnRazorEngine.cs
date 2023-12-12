using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;

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
    private readonly CodeRootFactory _codeRootFactory;
    private readonly LazySvc<DnnRazorSourceAnalyzer> _sourceAnalyzer;
    private readonly LazySvc<MyAppCodeLoader> _myAppCodeLoader;
    private readonly LazySvc<RoslynBuildManager> _roslynBuildManager;

    public DnnRazorEngine(MyServices helpers, CodeRootFactory codeRootFactory, LazySvc<CodeErrorHelpService> errorHelp, LazySvc<DnnRazorSourceAnalyzer> sourceAnalyzer, LazySvc<MyAppCodeLoader> myAppCodeLoader, LazySvc<RoslynBuildManager> roslynBuildManager) : base(helpers)
    {
        _myAppCodeLoader = myAppCodeLoader;
        ConnectServices(
            _codeRootFactory = codeRootFactory,
            _errorHelp = errorHelp,
            _sourceAnalyzer = sourceAnalyzer,
            _myAppCodeLoader = myAppCodeLoader,
            _roslynBuildManager = roslynBuildManager
        );
    }

    #endregion


    [PrivateApi]
    protected RazorComponentBase Webpage
    {
        get => Log.Getter(() => _webpage);
        set => Log.Setter(() => _webpage = value);
    }
    private RazorComponentBase _webpage;

    /// <inheritdoc />
    [PrivateApi]
    public override void Init(IBlock block)
    {
        var l = Log.Fn();
        base.Init(block);
        try
        {
            InitWebpage();
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
    protected HttpContextBase HttpContextCurrent => 
        _httpContext.Get(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
    private readonly GetOnce<HttpContextBase> _httpContext = new();

    [PrivateApi]
    private (TextWriter writer, List<Exception> exception) Render(TextWriter writer, object data)
    {
        var l = Log.Fn<(TextWriter writer, List<Exception> exception)>(message: "will render into TextWriter");
        var page = Webpage; // access the property once only
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
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            page.ExecutePageHierarchy(new WebPageContext(HttpContextCurrent, page, data), writer, page);
        }
        catch (Exception maybeIEntityCast)
        {
            var ex = l.Ex(_errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
            // Special form of throw to preserve details about the call stack
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // fake throw, just so the code shows what happens
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
        return l.Return((writer, page.SysHlp.ExceptionsOrNull));
    }

    [PrivateApi]
    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        if (args.Name == _appCodeAssembly?.FullName)
            return _appCodeAssembly;
        return null;
    }
    private Assembly _appCodeAssembly = null;

    [PrivateApi]
    protected override (string, List<Exception>) RenderImplementation(object data)
    {
        var l = Log.Fn<(string, List<Exception>)>();
        var writer = new StringWriter();
        var result = Render(writer, data);
        return l.ReturnAsOk((result.writer.ToString(), result.exception));
    }

    private object CreateWebPageInstance()
    {
        var l = Log.Fn<object>();
        object page = null;
        Type compiledType;
        var razorType = _sourceAnalyzer.Value.TypeOfVirtualPath(TemplatePath);
        try
        {
            _appCodeAssembly = _myAppCodeLoader.Value.GetAppCodeAssemblyOrNull(App.AppId);
            compiledType = razorType.MyApp 
                ? _roslynBuildManager.Value.GetCompiledType(TemplatePath, App.AppId)
                : BuildManager.GetCompiledType(TemplatePath);
        }
        catch (Exception compileEx)
        {
            // TODO: ADD MORE compile error help
            // 1. Read file
            // 2. Try to find base type - or warn if not found
            // 3. ...
            
            l.A($"Razor Type: {razorType}");
            var ex = l.Ex(_errorHelp.Value.AddHelpForCompileProblems(compileEx, razorType));
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


    private bool InitWebpage()
    {
        var l = Log.Fn<bool>();
        if (string.IsNullOrEmpty(TemplatePath)) return l.ReturnFalse("null path");

        var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
        // ReSharper disable once JoinNullCheckWithUsage
        if (objectValue == null)
            throw new InvalidOperationException($"The webpage found at '{TemplatePath}' was not created.");

        if (!(objectValue is RazorComponentBase pageToInit))
            throw new InvalidOperationException($"The webpage at '{TemplatePath}' must derive from RazorComponentBase.");
        Webpage = pageToInit;

        pageToInit.Context = HttpContextCurrent;
        pageToInit.VirtualPath = TemplatePath;
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
        return l.ReturnTrue("ok");
    }

    private void InitHelpers(RazorComponentBase webPage)
    {
        var l = Log.Fn();
        var dynCode = _codeRootFactory.BuildCodeRoot(webPage, Block, Log, compatibilityFallback: Constants.CompatibilityLevel9Old);
        webPage.ConnectToRoot(dynCode);
        l.Done();
    }

    /// <summary>
    /// Special old mechanism to always request jQuery and Rvt
    /// </summary>
    public bool OldAutoLoadJQueryAndRvt => Webpage._DynCodeRoot.Cdf.CompatibilityLevel <= Constants.MaxLevelForAutoJQuery;
}