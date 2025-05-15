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
/// This is a base class for dynamic code which is compiled at runtime.
///
/// > [!TIP]
/// > This is an old base class and works, but you should use a newer one such as <see cref="CodeTyped"/>
/// </summary>
/// <remarks>
/// The constructor cannot have parameters, otherwise inheriting code will run into problems.
/// </remarks>
[PrivateApi("Was public till v17")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class DynamicCode() : CustomCodeBase("Sxc.DynCod"), IHasCodeLog, IDynamicCode
{
    #region Constructor / Setup

    internal ICodeDynamicApiService CodeApi => field ??= _CodeApiSvc.GetDynamicApi();

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CodeHlp.CodeLog;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel10;

    #endregion

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

    /// <inheritdoc />
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true) =>
        CodeHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    #endregion

    #region Context, Settings, Resources

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi?.CmsContext;

    #endregion CmsContext

    #region AsDynamic and AsEntity

    /// <inheritdoc />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi?.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc />
    public dynamic AsDynamic(IEntity entity) => CodeApi?.Cdf.CodeAsDyn(entity);

    /// <inheritdoc />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi?.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => CodeApi?.Cdf.MergeDynamic(entities);

    /// <inheritdoc />
    public IEntity AsEntity(object dynamicEntity) => CodeApi?.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc />
    public IEnumerable<dynamic> AsList(object list) => CodeApi?.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource

    /// <inheritdoc />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    /// <inheritdoc />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);


    #endregion

    #region AsAdam

    /// <inheritdoc />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi?.AsAdam(item, fieldName);

    #endregion
}