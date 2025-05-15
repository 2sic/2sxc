using ToSic.Eav.Context;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Integration.Installation;

internal class BasicPlatformAppInstaller(WarnUseOfUnknown<BasicPlatformAppInstaller> _) : ServiceBase($"{LogScopes.NotImplemented}.Instll"), IIsUnknown, IPlatformAppInstaller
{
    public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp) => "mvc not implemented #todo #mvc";
}