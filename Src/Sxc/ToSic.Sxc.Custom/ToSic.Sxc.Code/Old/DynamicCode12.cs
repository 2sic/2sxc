using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.CodeApi.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

/// <summary>
/// Base class for v12 Dynamic Code
/// Adds new properties and methods, and doesn't keep old / legacy APIs
///
/// > [!TIP]
/// > This is an old base class and works, but you should use a newer one such as <see cref="CodeTyped"/>
/// </summary>
/// <remarks>
/// The constructor cannot have parameters, otherwise inheriting code will run into problems.
/// </remarks>
[PrivateApi("Was public till v17")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class DynamicCode12(): CustomCodeBase("Sxc.DynCod"), IHasCodeLog, IDynamicCode, IDynamicCode12
{
    #region Constructor / Setup

    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();


    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CodeHlp.CodeLog;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

    #endregion

    #region Stuff added by Code12

    /// <inheritdoc cref="IDynamicCode12.Convert" />
    public IConvertService Convert => field ??= CodeApi.Convert;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => CodeApi?.Resources;

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => CodeApi?.Settings;

    [PrivateApi("Not yet ready")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IDevTools DevTools => CodeApi.DevTools;

    #endregion

    // Stuff "inherited" from DynamicCode (old base class)

    #region App / Data / Content / Header

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => CodeApi?.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IDataSource Data => CodeApi?.Data;

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => CodeApi?.Content;
    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => CodeApi?.Header;

    #endregion



    #region Link and Edit
    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi?.Link;
    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => CodeApi?.Edit;

    #endregion

    #region SharedCode - must also map previous path to use here

    /// <inheritdoc />
    [PrivateApi]
    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="IDynamicCode.CreateInstance(string, NoParamOrder, string, string, bool)" />
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true) =>
        CodeHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    #endregion

    #region Context, Settings, Resources

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi?.CmsContext;

    #endregion CmsContext

    #region AsDynamic and AsEntity

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi?.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => CodeApi?.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi?.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => CodeApi?.Cdf.MergeDynamic(entities);

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi?.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi?.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);


    #endregion

    #region AsAdam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi?.AsAdam(item, fieldName);

    #endregion

}