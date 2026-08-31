using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.Install;

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

    public AppInstallation(Dependencies services, LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        : base(services, "Sxc.AppInstall", connect: [install, context])
    {
        ProvideOutRaw(() => Settings(install, context), name: "Settings", options: Options);
        ProvideOutRaw(() => InstalledApps(install, context), name: "InstalledApps", options: Options);
        ProvideOutRaw(() => Rules(install, context), name: "Rules", options: Options);
    }

    private InstallAppsDto Result(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => install.Value.InstallSettings(IsContentApp, context.BlockRequired().Context.Module);

    private IEnumerable<InstallationSettingsRaw> Settings(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => [new(Result(install, context).remoteUrl)];
    private IEnumerable<InstalledAppRaw> InstalledApps(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => Result(install, context).installedApps.Select(app => new InstalledAppRaw(app));
    private IEnumerable<InstallRuleRaw> Rules(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => Result(install, context).rules?.Select(rule => new InstallRuleRaw(rule)) ?? [];
    private static DataFactoryOptions Options() => new();

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
}
