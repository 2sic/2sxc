using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Sys;

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
public partial class AppForCodeShared(AppForCodeShared.Dependencies services, string? logName = default, object[]? connect = default)
    : ServiceWithSetup<AppForCodeShared.Dependencies, AppForCodeShared.Options>(services, logName ?? "Eav.App", connect: connect),
        IAppForCode
{
    /// <inheritdoc />
    public int ZoneId => MyOptions.AppIdentity.ZoneId;

    /// <inheritdoc />
    public int AppId => MyOptions.AppIdentity.AppId;

    #region AppReader and all derived properties of the AppReader

    public IAppReader AppReader => field
        ??= Services.AppReaders.Get(this) ?? throw new ("AppReaderInt not set, did you call Init()?");

    public IAppSpecs AppSpecs => field ??= AppReader.Specs;

    IAppPaths IAppForCode.AppPaths => field ??= Services.AppPathSvc.Get(AppReader, AppSite);

    #region Settings, Config, Metadata

    IEntity? IAppForCode.SettingsEntity => AppSpecs.Settings.MetadataItem;
    IEntity? IAppForCode.ResourcesEntity => AppSpecs.Resources.MetadataItem;

    AppForCodeDataSourceService IAppForCode.DataService => field ??= Services.AppDataSvc.New(new(this, MyOptions.DataSpecs));

    #endregion

    #endregion


    public override void Setup(Options options)
    {
        var (replaceSite, appIdentity, dataSpecs) = options;
        var l = Log.Fn($"prep App #{appIdentity.Show()}, hasSite: {replaceSite != null} hasDataSpecs:{dataSpecs != null}");
        base.Setup(options);


        // If we have a replacement site (like App being used from another site), set it here
        if (replaceSite != null)
            AppSite = replaceSite;

        // Env / Tenant must be re-checked here
        if (AppSite == null)
            throw new("no site/portal received");

        // in case the DI gave a bad tenant, try to look up
        if (AppSite.Id == EavConstants.NullId
            && appIdentity.AppId != EavConstants.NullId
            && appIdentity.AppId != AppConstants.AppIdNotFound)
            AppSite = Services.ZoneMapper.SiteOfApp(appIdentity.AppId);
        
        l.Done();
    }

    #region Environment & Physical Path

    internal ISite AppSite { get; set; } = services.Site;


    #endregion
}