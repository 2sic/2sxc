using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Metadata;
using ToSic.Eav.Sys;
using ToSic.Sys.Caching.PiggyBack;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// A <em>single-use</em> app-object providing quick simple api to access
/// name, folder, data, metadata etc.
/// </summary>
/// <remarks>
/// * was `ToSic.Eav.Apps.App` till ca. 2023-01-11 ca. v16.09
/// * then was `ToSic.Eav.Apps.Internal.EavApp` till v20
/// </remarks>
/// <param name="services">All the dependencies of this app, managed by this app</param>
/// <param name="logName">must be null by default, because of DI</param>
[PrivateApi("Hide implementation - was PublicApi_Stable_ForUseInYourCode till 16.09")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class SxcAppBase(AppForCodeShared.Dependencies services, string? logName = default, object[]? connect = default)
    : ServiceWithSetup<AppForCodeShared.Dependencies, AppForCodeShared.Options>(services, logName ?? "Eav.App", connect: connect),
        IAppForCode
{
    /// <inheritdoc />
    public int ZoneId => MyOptions.AppIdentity.ZoneId;

    /// <inheritdoc />
    public int AppId => MyOptions.AppIdentity.AppId;

    #region AppReader and all derived properties of the AppReader

    private IAppReader AppReader => field
        ??= Services.AppReaders.Get(this) ?? throw new ("AppReaderInt not set, did you call Init()?");

    IAppReader IAppForCode.AppReader => AppReader;

    IAppSpecs IAppForCode.AppSpecs => field ??= AppReader.Specs;

    IAppPaths IAppForCode.AppPaths => field ??= Services.AppPathSvc.Get(AppReader, MySite);

    private IAppForCode NewSource => field ??= this;

    #region Settings, Config, Metadata

    IEntity? IAppForCode.SettingsEntity => NewSource.AppSpecs.Settings.MetadataItem;
    IEntity? IAppForCode.ResourcesEntity => NewSource.AppSpecs.Resources.MetadataItem;

    SxcAppDataService IAppForCode.DataService => field ??= Services.AppDataSvc.New(new(this, MyOptions.DataSpecs));

    #endregion

    #endregion


    public override void Setup(AppForCodeShared.Options options)
    {
        var (replaceSite, appIdentity, dataSpecs) = options;
        var l = Log.Fn($"prep App #{appIdentity.Show()}, hasSite: {replaceSite != null} hasDataSpecs:{dataSpecs != null}");
        base.Setup(options);


        // If we have a replacement site (like App being used from another site), set it here
        if (replaceSite != null)
            MySite = replaceSite;

        // Env / Tenant must be re-checked here
        if (MySite == null)
            throw new("no site/portal received");

        // in case the DI gave a bad tenant, try to look up
        if (MySite.Id == EavConstants.NullId
            && appIdentity.AppId != EavConstants.NullId
            && appIdentity.AppId != AppConstants.AppIdNotFound)
            MySite = Services.ZoneMapper.SiteOfApp(appIdentity.AppId);
        
        l.Done();
    }

    #region Environment & Physical Path

    protected ISite MySite { get; set; } = services.Site;


    #endregion
}

public class AppForCodeShared
{
    /// <summary>
    /// Options
    /// </summary>
    /// <param name="ReplaceSite"></param>
    /// <param name="AppIdentity"></param>
    /// <param name="DataSpecs"></param>
    public record Options(ISite? ReplaceSite, IAppIdentityPure AppIdentity, AppDataConfigSpecs? DataSpecs);

    /// <summary>
    /// Dependencies
    /// </summary>
    [PrivateApi]
    public record Dependencies(
        IZoneMapper ZoneMapper,
        ISite Site,
        IAppReaderFactory AppReaders,
        IAppPathsMicroSvc AppPathSvc,
        Generator<SxcAppDataService, SxcAppDataService.Options> AppDataSvc)
        : DependenciesBase(connect: [ZoneMapper, Site, AppReaders, AppPathSvc, AppDataSvc]);
}