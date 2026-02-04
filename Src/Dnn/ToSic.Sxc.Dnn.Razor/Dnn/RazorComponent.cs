using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// The base class for Razor-Components in 2sxc 10+ to 2sxc 11 - deprecated now<br/>
/// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
/// </summary>
[PublicApi("...but deprecated! use Razor14, RazorTyped or newer")]
public abstract partial class RazorComponent : RazorComponentBase,
    IDynamicCode,
    IHasDnn,
    // previous
    IDnnRazorCompatibility,
    IDnnRazor11,
    ICreateInstance,
    // new
    IHasCodeHelp
{
    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    /// <inheritdoc />
    public IDnnContext Dnn => (ExCtx as IHasDnn)?.Dnn;

    public const string NotImplementedUseCustomBase = "Use a newer base class like Custom.Hybrid.Razor12 or Custom.Dnn.Razor12 to leverage this.";

    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public override ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => RzrHlp.Html;

    #endregion

    #region Link, Edit, Dnn, App, Data

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi.Link;

    /// <inheritdoc cref="IDynamicCodeDocs.Edit" />
    public IEditService Edit => CodeApi.Edit;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel10;

    /// <inheritdoc />
    public new IApp App => CodeApi.App;

    #endregion

    #region Data - with old interface #DataInAddWontWork

    [PrivateApi]
    public IDataSource Data => /*(IBlockDataSource)*/CodeApi.Data;

    //// This is explicitly implemented so the interfaces don't complain
    //// but actually we're not showing this - in reality we're showing the Old (see above)
    //IDataSource IDynamicCode.Data => CodeApi.Data;

    #endregion


    #region AsDynamic in many variations

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCodeDocs.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi.Cdf.CodeAsDynList(list);

    #endregion


    #region Data Source Stuff

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    #endregion


    #region Content, Header, etc. and List
    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    public dynamic Content => CodeApi.Content;

    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    public dynamic Header => CodeApi.Header;

    #endregion


    #region Adam 

    /// <inheritdoc cref="IDynamicCodeDocs.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    #endregion

    #region CmsContext

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc />
    public virtual dynamic CreateInstance(string virtualPath, NoParamOrder npo = default, string name = null, string relativePath = null, bool throwOnError = true)
        => RzrHlp.CreateInstance(virtualPath: virtualPath, name: name, throwOnError: throwOnError);

    #endregion

    // Added this in v20 to show uses of GetBestValue; but much of it may not be applicable, in which case we should create a separate list for SexyContentWebPage and Dnn.RazorComponent
    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpDbRazor.CompileRazorOrCode12;

}