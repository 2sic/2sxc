using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.Install;
using ToSic.Sxc.Backend.SysData;

namespace ToSic.Sxc.Backend.Sys;

[PrivateApi]
[VisualQuery(NiceName = "App Installation", NameId = "187dfef4-13a5-4aed-a806-f5756f424176", NameIds = ["System.AppInstallation"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.System, UiHint = "Settings and state used by the app installer")]
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

    private IEnumerable<IRawEntity> Settings(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => [new RawEntity { Values = new Dictionary<string, object?> { ["RemoteUrl"] = Result(install, context).remoteUrl } }];
    private IEnumerable<IRawEntity> InstalledApps(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => SysDataRaw.Many(Result(install, context).installedApps);
    private IEnumerable<IRawEntity> Rules(LazySvc<InstallControllerReal> install, ISxcCurrentContextService context)
        => SysDataRaw.Many(Result(install, context).rules);
    private static DataFactoryOptions Options() => new() { AutoId = true, AllowUnknownValueTypes = true };
}
