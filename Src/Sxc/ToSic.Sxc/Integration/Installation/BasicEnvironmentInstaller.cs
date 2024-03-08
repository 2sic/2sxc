using ToSic.Eav.Context;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Integration.Installation;

internal class BasicEnvironmentInstaller(WarnUseOfUnknown<BasicEnvironmentInstaller> _) : ServiceBase($"{LogScopes.NotImplemented}.Instll"), IEnvironmentInstaller, IIsUnknown, IPlatformAppInstaller
{
    // for now, always assume installation worked
    public string UpgradeMessages() => null;

    // don't do anything for now
    public bool ResumeAbortedUpgrade() => true;

    public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp) => "mvc not implemented #todo #mvc";
}