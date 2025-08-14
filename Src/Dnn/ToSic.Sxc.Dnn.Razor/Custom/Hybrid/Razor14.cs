using Custom.Razor.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v14 Dynamic Razor files.
/// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
/// This contains all the popular services used in v14, so that your code can be lighter. 
/// </summary>
/// <remarks>
/// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
/// </remarks>
[PublicApi]
public abstract partial class Razor14: RazorComponentBase, IRazor14<object, ServiceKit14>, IHasCodeHelp, ICreateInstance
{
    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    /// <inheritdoc cref="DnnRazorHelper.RenderPageNotSupported"/>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override HelperResult RenderPage(string path, params object[] data)
        => RzrHlp.RenderPageNotSupported();


    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;


    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();


    public ServiceKit14 Kit => field ??= CodeApi.ServiceKit14;


    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public override ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => RzrHlp.Html;

    #endregion


    #region Link, Edit

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi.Link;

    /// <inheritdoc cref="IDynamicCodeDocs.Edit" />
    public IEditService Edit => CodeApi.Edit;

    #endregion


    #region CmsContext

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    #endregion


    #region Content, Header, etc. and List

    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    public dynamic Content => CodeApi.Content;

    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    public dynamic Header => CodeApi.Header;

    /// <inheritdoc />
    public IDataSource Data => CodeApi.Data;

    #endregion

    #region CreateSource Stuff

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    #endregion



    #region Dev Tools & Dev Helpers

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeApi.DevTools;

    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpDbRazor.CompileRazorOrCode14;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc />
    public virtual dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => RzrHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError: throwOnError);

    /// <inheritdoc cref="ITypedCode16.GetCode"/>
    [PrivateApi("added in 16.05, but not sure if it should be public")]
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

    #endregion

}