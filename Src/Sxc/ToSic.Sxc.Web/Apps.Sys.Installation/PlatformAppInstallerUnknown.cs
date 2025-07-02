using ToSic.Eav.Context;
using ToSic.Sxc.Context;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Apps.Sys.Installation;

internal class PlatformAppInstallerUnknown(WarnUseOfUnknown<PlatformAppInstallerUnknown> _)
    : ServiceBase($"{LogScopes.NotImplemented}.Instll"), IIsUnknown, IPlatformAppInstaller
{
    public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp) => "mvc not implemented #todo #mvc";
}