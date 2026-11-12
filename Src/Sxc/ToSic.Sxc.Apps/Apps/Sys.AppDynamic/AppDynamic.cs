using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Decorators;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.DataService.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Apps.Sys.AppDynamic;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
/// <remarks>
/// * Originally called `ToSic.Sxc.Apps.App` and was marked as `PublicApi_Stable_ForUseInYourCode` up to v16.03.
/// * Changed name in v22 to `AppDynamic` to clarify what it actually is
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDynamic(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryBuilder<Query>> queryManager, LazySvc<AppDynamicQueries> appDynamicQueries)
    // Note: If this is ever changed to not inherit from the EavApp, make sure you correct/update the LightSpeed code as well as it checks for this base class
    : ServiceWithSetup<IAppForCode>("App.SxcApp", connect: [globalPaths, queryManager, appDynamicQueries]),
        IAppDynamic,
        IWrapper<IAppForCode>,
        INeedsExecutionContext
{
    #region Setup Execution Context - for Cdf and data wrapping to DynamicEntity

    public void ConnectToRoot(IExecutionContext exCtx) => ExCtx = exCtx;

    private IExecutionContext? ExCtx { get; set; }

    private ICodeDataFactory Cdf => field ??= (ExCtx ?? throw new InvalidOperationException("ExecutionContext is not set")).GetCdf();

    #endregion



    #region Identity

    public int ZoneId => MyOptions.ZoneId;

    public int AppId => MyOptions.AppId;


    #endregion



    #region IApp (old dynamic API) Properties

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    string IAppDynamic.Name => field ??= MyOptions.AppSpecs.Name;

    /// <inheritdoc />
    [Obsolete]
    [PrivateApi]
    string IAppDynamic.AppGuid => MyOptions.AppSpecs.NameId;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    string IAppDynamic.NameId => field ??= MyOptions.AppSpecs.NameId;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    string IAppDynamic.Folder => field ??= MyOptions.AppSpecs.Folder;

    #endregion



    #region IApp Paths

    /// <inheritdoc cref="IAppDynamic.Path" />
    public string Path => field ??= MyOptions.AppPaths.Path;


    /// <inheritdoc cref="IAppDynamic.Thumbnail" />
    string? IAppDynamic.Thumbnail => _thumbnail.Get(() => AppThumbnailHelper.GetUrl(MyOptions.AppReader, MyOptions.AppPaths, globalPaths));
    private readonly LazyGet<string?> _thumbnail = new();

    /// <inheritdoc cref="IAppDynamic.PathShared" />
    public string PathShared => field ??= MyOptions.AppPaths.PathShared;

    [PrivateApi]
    public string PhysicalPath
        => MyOptions.AppPaths.PhysicalPath;

    /// <inheritdoc cref="IAppDynamic.PhysicalPathShared" />
    public string PhysicalPathShared => field ??= MyOptions.AppPaths.PhysicalPathShared;

    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePath => field ??= MyOptions.AppPaths.RelativePath;

    [PrivateApi("not public, not sure if we should surface this")]
    public string RelativePathShared => field ??= MyOptions.AppPaths.RelativePathShared;


    #endregion



    #region Metadata and Permission Accessors - only in IApp (classic), not in IAppTyped or IAppForCode

    /// <inheritdoc />
    public IMetadata Metadata => MyOptions.AppSpecs.Metadata;

    #endregion



    #region Dynamic Properties: Configuration, Settings, Resources

    // Create config object. Note that AppConfiguration could be null, then it would use default values
    /// <inheritdoc />
    IAppConfiguration IAppDynamic.Configuration => MyOptions.AppSpecs.Configuration;

    /// <inheritdoc/>
    dynamic? IAppDynamic.Settings => MyOptions.SettingsEntity == null
        ? null
        : _settings.Get(() => MakeDynProperty(MyOptions.SettingsEntity, propsRequired: false));
    private readonly LazyGet<dynamic> _settings = new();

    /// <inheritdoc/>
    dynamic? IAppDynamic.Resources => MyOptions.ResourcesEntity == null
        ? null
        : _res.Get(() => MakeDynProperty(MyOptions.ResourcesEntity, propsRequired: false));
    private readonly LazyGet<dynamic> _res = new();

    private IDynamicEntity MakeDynProperty(IEntity entity, bool propsRequired)
        => Cdf.AsDynamic(CmsEditDecorator.Wrap(entity, false), new() { ItemIsStrict = propsRequired });

    #endregion



    #region Data

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    IAppData IAppDynamic.Data => field ??= MyOptions.DataService.BuildData<AppDataDynamic, IAppData>(this.PureIdentity());

    #endregion



    #region Query

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public IDictionary<string, IQuery> Query => field
        ??= appDynamicQueries.Value.AllQueries(this,
            MyOptions.DataService.LookUpEngine
            ?? throw new("Can't use app-queries, because the necessary configuration provider hasn't been initialized. Call InitData first."));

    /// <inheritdoc />
    IQuery IAppDynamic.GetQuery(string name)
    {
        // Try to find the local query, abort if not found
        // Not final implementation, but goal is to properly cascade from AppState to parent
        if (Query.TryGetValue(name, out var query) && query != null)
            return query;

        // Try to find query definition - while also checking parent apps
        var qEntity = queryManager.Value.TryBuild(MyOptions.AppReader, name, MyOptions.DataService.LookUpEngine, recurseParents: 3)
                      ?? throw new((DataSourceConstantsInternal.IsGlobalQuery(name) ? "Global " : "") + "Query not Found!");

        return qEntity;
    }

    #endregion



    #region IAppForCode implementation - for re-use in certain APIs

    public IAppForCode? GetContents() => MyOptions;

    #endregion

}