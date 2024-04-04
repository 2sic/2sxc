using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// This is the base class for custom code (.cs) files in your Apps.
/// By inheriting from this base class, you will automatically have the context like the App object etc. available.
///
/// > [!TIP]
/// > This is an old base class and works, but you should use a newer one such as <see cref="CodeTyped"/>
/// </summary>
[PublicApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class Code12 : CustomCodeBase, IHasCodeLog, IDynamicCode, IDynamicCode12
{
    #region Constructor / Setup

    /// <summary>
    /// Main constructor. May never have parameters, otherwise inheriting code will run into problems. 
    /// </summary>
    [PrivateApi]
    protected Code12() : base("Sxc.Code12") { }

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CodeHlp.CodeLog;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => _CodeApiSvc.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

    #endregion

    #region Stuff added by Code12

    /// <inheritdoc cref="IDynamicCode12.Convert" />
    public IConvertService Convert => _CodeApiSvc.Convert;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => _CodeApiSvc?.Resources;

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => _CodeApiSvc?.Settings;

    [PrivateApi("Not yet ready")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IDevTools DevTools => _CodeApiSvc.DevTools;

    #endregion

    // Stuff "inherited" from DynamicCode (old base class)

    #region App / Data / Content / Header

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => _CodeApiSvc?.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IBlockInstance Data => _CodeApiSvc?.Data;

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => _CodeApiSvc?.Content;
    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => _CodeApiSvc?.Header;

    #endregion



    #region Link and Edit

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc?.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => _CodeApiSvc?.Edit;

    #endregion

    #region SharedCode - must also map previous path to use here

    /// <inheritdoc />
    [PrivateApi]
    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="IDynamicCode.CreateInstance" />
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true) =>
        CodeHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    #endregion

    #region Context, Settings, Resources

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => _CodeApiSvc?.CmsContext;

    #endregion CmsContext

    #region AsDynamic and AsEntity

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => _CodeApiSvc?.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => _CodeApiSvc?.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => _CodeApiSvc?.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => _CodeApiSvc?.Cdf.MergeDynamic(entities);

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => _CodeApiSvc?.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => _CodeApiSvc?.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(source);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(inSource, configurationProvider);


    #endregion

    #region AsAdam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => _CodeApiSvc?.AsAdam(item, fieldName);

    #endregion

}