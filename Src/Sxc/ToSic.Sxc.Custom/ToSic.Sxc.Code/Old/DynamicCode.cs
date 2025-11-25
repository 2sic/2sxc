using Custom.Hybrid;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

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

    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CompileCodeHlp.CodeLog;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel10;

    #endregion

    #region App / Data / Content / Header

    /// <inheritdoc cref="IDynamicCodeDocs.App" />
    public IApp App => CodeApi.App;

    /// <inheritdoc cref="IDynamicCodeDocs.Data" />
    public IDataSource Data => CodeApi.Data;

    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    public dynamic? Content => CodeApi.Content;
    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    public dynamic? Header => CodeApi.Header;

    #endregion


    #region Link and Edit

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi.Link;
    /// <inheritdoc cref="IDynamicCodeDocs.Edit" />
    public IEditService Edit => CodeApi.Edit;

    #endregion

    #region SharedCode - must also map previous path to use here

    /// <inheritdoc />
    [PrivateApi]
    string IGetCodePath.CreateInstancePath { get; set; } = null!;

    /// <inheritdoc />
    public dynamic? CreateInstance(string virtualPath, NoParamOrder npo = default, string? name = null, string? relativePath = null, bool throwOnError = true) =>
        CompileCodeHlp.CreateInstance(virtualPath, npo, name, relativePath, throwOnError);

    #endregion

    #region Context, Settings, Resources

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    #endregion CmsContext

    #region AsDynamic and AsEntity

    /// <inheritdoc />
    public dynamic? AsDynamic(string json, string? fallback = default) => CodeApi.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc />
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc />
    public dynamic? AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12Docs.AsDynamic(object[])" />
    public dynamic? AsDynamic(params object[] entities) => CodeApi.Cdf.MergeDynamic(entities);

    /// <inheritdoc />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc />
    public IEnumerable<dynamic> AsList(object list) => CodeApi.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource

    /// <inheritdoc />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    /// <inheritdoc />
    public T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);


    #endregion

    #region AsAdam

    /// <inheritdoc />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    #endregion
}