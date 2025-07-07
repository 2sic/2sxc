#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Sys.Integration.Installation;

internal class EnvironmentInstallerUnknown(WarnUseOfUnknown<EnvironmentInstallerUnknown> _) : ServiceBase($"{LogScopes.NotImplemented}.Instll"), IEnvironmentInstaller, IIsUnknown
{
    // for now, always assume installation worked
    public string UpgradeMessages() => null!;

    // don't do anything for now
    public bool ResumeAbortedUpgrade() => true;
}