using ToSic.Eav.Context;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Integration;
using ToSic.Eav.Services;

// NOTE 2023-01-11 refactoring - was previously ToSic.Eav.Apps.App - renamed to ToSic.Eav.Apps.Internal.EavApp
// Could be a breaking change

namespace ToSic.Eav.Apps.Internal;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
/// <param name="services">All the dependencies of this app, managed by this app</param>
/// <param name="logName">must be null by default, because of DI</param>
[PrivateApi("Hide implementation - was PublicApi_Stable_ForUseInYourCode till 16.09")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class EavApp(EavApp.MyServices services, string logName = default, object[] connect = default)
    : AppBase<EavApp.MyServices>(services, logName ?? "Eav.App", connect: connect), IApp
{
    // ReSharper disable once InconsistentNaming
    private readonly MyServices services = services;

    #region Constructor / DI

    /// <summary>
    /// Helper class, so inheriting stuff doesn't need to update the constructor all the time
    /// </summary>
    [PrivateApi]
    public class MyServices(
        IZoneMapper zoneMapper,
        ISite site,
        IAppReaderFactory appReaders,
        IDataSourcesService dataSourceFactory,
        LazySvc<QueryManager> queryManager,
        IAppDataConfigProvider dataConfigProvider)
        : MyServicesBase(connect: [zoneMapper, site, appReaders, dataSourceFactory, queryManager, dataConfigProvider])
    {
        public IAppDataConfigProvider DataConfigProvider { get; } = dataConfigProvider;
        public LazySvc<QueryManager> QueryManager { get; } = queryManager;
        internal IZoneMapper ZoneMapper { get; } = zoneMapper;
        internal ISite Site { get; } = site;
        internal IAppReaderFactory AppReaders { get; } = appReaders;
        internal IDataSourcesService DataSourceFactory { get; } = dataSourceFactory;
    }

    #endregion

    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public string Folder { get; private set; }

    /// <inheritdoc />
    public string NameId { get; private set; }

    /// <inheritdoc />
    [Obsolete]
    [PrivateApi]
    public string AppGuid => NameId;

    public EavApp Init(ISite site, IAppIdentityPure appIdentity, AppDataConfigSpecs dataSpecs)
    {
        var l = Log.Fn<EavApp>();

        // 2024-03-18 moved here...
        if (site != null) Site = site;

        // Env / Tenant must be re-checked here
        if (Site == null) throw new("no site/portal received");
            
        // in case the DI gave a bad tenant, try to look up
        if (Site.Id == Constants.NullId
            && appIdentity.AppId != Constants.NullId
            && appIdentity.AppId != AppConstants.AppIdNotFound)
            Site = Services.ZoneMapper.SiteOfApp(appIdentity.AppId);

        InitAppBaseIds(appIdentity);
        l.A($"prep App #{appIdentity.Show()}, has{nameof(dataSpecs)}:{dataSpecs != null}");

        // Look up name in cache
        AppReaderInt = services.AppReaders.Get(this);
        NameId = AppReaderInt.Specs.NameId;

        InitializeResourcesSettingsAndMetadata();

        // for deferred initialization as needed
        _dataConfigSpecs = dataSpecs;

        return l.Return(this);
    }
}