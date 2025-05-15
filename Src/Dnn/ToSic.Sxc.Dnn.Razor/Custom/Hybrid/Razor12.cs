using ToSic.Eav.LookUp;
using ToSic.Lib.Code.Help;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.CodeApi.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Dnn.Razor;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// The base class for Hybrid Razor-Components in 2sxc 12 <br/>
/// Provides context objects like CmsContext, helpers like Edit and much more. <br/>
/// </summary>
[PublicApi]
public abstract partial class Razor12 : RazorComponentBase, IRazor12, IHasCodeHelp, ICreateInstance
{
    internal ICodeDynamicApiHelper CodeApi => field ??= _CodeApiSvc.GetDynamicApi();

    /// <inheritdoc cref="DnnRazorHelper.RenderPageNotSupported"/>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override HelperResult RenderPage(string path, params object[] data) 
        => RzrHlp.RenderPageNotSupported();

    #region Core Properties which should appear in docs

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public override ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => RzrHlp.Html;

    #endregion


    #region Link, Edit, Dnn, App, Data

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => CodeApi.Edit;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

    /// <inheritdoc />
    public new IApp App => CodeApi.App;

    /// <inheritdoc />
    public IDataSource Data => CodeApi.Data;

    #endregion

    #region AsDynamic in many variations

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    [PublicApi("Careful - still Experimental in 12.02")]
    public dynamic AsDynamic(params object[] entities) => CodeApi.Cdf.MergeDynamic(entities);

    #endregion

    #region AsEntity
    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);
    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi.Cdf.CodeAsDynList(list);

    #endregion

    #region Convert-Service

    /// <inheritdoc />
    public IConvertService Convert => field ??= CodeApi.Convert;

    #endregion


    #region Data Source Stuff

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    #endregion

    #region Content, Header, etc. and List

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => CodeApi.Content;

    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => CodeApi.Header;

    #endregion





    #region Adam 

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    #endregion

    #region v11 properties CmsContext

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;
    #endregion

    #region v12 properties Resources, Settings, Path

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => CodeApi.Resources;

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => CodeApi.Settings;

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeApi.DevTools;

    ///// <inheritdoc />
    //public string Path => VirtualPath;

    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpForRazor12.Compile12;

    #endregion

    #region CreateInstance

    /// <inheritdoc cref="ICreateInstance.CreateInstancePath"/>
    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
    public virtual dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => RzrHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError: throwOnError);

    #endregion

}