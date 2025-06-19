using System.Diagnostics.CodeAnalysis;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Metadata;
using ToSic.Eav.Services;
using ToSic.Eav.Sys;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
/// <remarks>
/// * was `ToSic.Eav.Apps.App` till ca. 2023-01-11 ca. v16.09
/// * then was `ToSic.Eav.Apps.Internal.EavApp` v20
/// </remarks>
/// <param name="services">All the dependencies of this app, managed by this app</param>
/// <param name="logName">must be null by default, because of DI</param>
[PrivateApi("Hide implementation - was PublicApi_Stable_ForUseInYourCode till 16.09")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class SxcAppBase(SxcAppBase.MyServices services, string? logName = default, object[]? connect = default)
    : AppBase<SxcAppBase.MyServices>(services, logName ?? "Eav.App", connect: connect),
        IAppWithInternal
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

    #region AppReader and all derived properties of the AppReader

    protected internal IAppReader AppReaderInt
    {
        get => field ?? throw new("AppReaderInt not set, did you call Init()?");
        private set;
    } = null!;

    IAppReader IAppWithInternal.AppReader => AppReaderInt;


    [field: AllowNull, MaybeNull]
    private IAppSpecs AppSpecs => field ??= AppReaderInt.Specs;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string Name => field ??= AppReaderInt.Specs.Name; // ?? KnownAppsConstants.ErrorAppName;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string Folder => field ??= AppReaderInt.Specs.Folder; // ?? KnownAppsConstants.ErrorAppName;

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string NameId => field ??= AppReaderInt.Specs.NameId;

    /// <inheritdoc />
    [Obsolete]
    [PrivateApi]
    public string AppGuid => NameId;

    #region Metadata and Permission Accessors

    /// <inheritdoc />
    public IMetadataOf Metadata => AppSpecs.Metadata;

    /// <summary>
    /// Permissions of this app
    /// </summary>
    public IEnumerable<IPermission> Permissions => Metadata.Permissions;

    #endregion

    #region Settings, Config, Metadata

    public IEntity? AppSettings => AppSpecs.Settings.MetadataItem;
    public IEntity? AppResources => AppSpecs.Resources.MetadataItem;

    #endregion

    #endregion


    public SxcAppBase Init(ISite site, IAppIdentityPure appIdentity, AppDataConfigSpecs dataSpecs)
    {
        var l = Log.Fn<SxcAppBase>();

        // 2024-03-18 moved here...
        if (site != null)
            Site = site;

        // Env / Tenant must be re-checked here
        if (Site == null) throw new("no site/portal received");
            
        // in case the DI gave a bad tenant, try to look up
        if (Site.Id == EavConstants.NullId
            && appIdentity.AppId != EavConstants.NullId
            && appIdentity.AppId != AppConstants.AppIdNotFound)
            Site = Services.ZoneMapper.SiteOfApp(appIdentity.AppId);

        InitAppBaseIds(appIdentity);
        l.A($"prep App #{appIdentity.Show()}, has{nameof(dataSpecs)}:{dataSpecs != null}");

        // Look up name in cache
        AppReaderInt = services.AppReaders.Get(this);

        //InitializeResourcesSettingsAndMetadata();

        // for deferred initialization as needed
        _dataConfigSpecs = dataSpecs;

        return l.Return(this);
    }
}