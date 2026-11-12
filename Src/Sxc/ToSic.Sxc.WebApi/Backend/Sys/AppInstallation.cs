using ToSic.Eav.Apps.Install.Sys;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Backend.Sys;

[PrivateApi]
[VisualQuery(
    NiceName = "App Installation",
    NameId = "187dfef4-13a5-4aed-a806-f5756f424176",
    NameIds = ["System.AppInstallation"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "Settings and state used by the app installer")]
public class AppInstallation : CustomDataSource
{
    [Configuration]
    public bool IsContentApp => Configuration.GetThis(false);

    public AppInstallation(Dependencies services, LazySvc<InstallSettingsSvc> install)
        : base(services, "Sxc.AppInstall", connect: [install])
    {
        var result = new Lazy<InstallAppsDto>(() => install.Value.InstallSettings(IsContentApp));

        ProvideOutRaw(name: "Settings", data: () => Settings(result.Value), options: Options);
        ProvideOutRaw(name: "InstalledApps", data: () => InstalledApps(result.Value), options: Options);
        ProvideOutRaw(name: "Rules", data: () => Rules(result.Value), options: Options);
    }

    private IEnumerable<InstallationSettingsRaw> Settings(InstallAppsDto result)
        => [new InstallationSettingsRaw(result.remoteUrl)];
    private IEnumerable<InstalledAppRaw> InstalledApps(InstallAppsDto result)
        => result.installedApps.Select(app => new InstalledAppRaw(app));
    private IEnumerable<InstallRuleRaw> Rules(InstallAppsDto result)
        => result.rules?.Select(rule => new InstallRuleRaw(rule)) ?? [];

    private static DataFactoryOptions Options() => new();

    // ReSharper disable NotAccessedPositionalProperty.Local
    private sealed record InstallationSettingsRaw(string RemoteUrl) : IRawEntityAutoConvert;

    private sealed class InstalledAppRaw(AppDtoLight app) : IRawEntityAutoConvert
    {
        [ContentTypeTitle]
        public string Name => app.name;
        public string AppGuid => app.guid;
        public string Version => app.version;
    }

    private sealed class InstallRuleRaw(AppInstallRuleDto rule) : IRawEntityAutoConvert
    {
        [ContentTypeTitle]
        public string Name => rule.name;
        public string AppGuid => rule.appGuid;
        public string Mode => rule.mode;
        public string Target => rule.target;
        public string Url => rule.url;
    }
    // ReSharper restore NotAccessedPositionalProperty.Local
}
