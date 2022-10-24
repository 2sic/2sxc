using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Run
{
    public interface IEnvironmentInstaller: IHasLog<IEnvironmentInstaller>
    {
        /// <summary>
        /// Get upgrade messages to show to the user if the upgrade/install needs attention
        /// </summary>
        /// <returns></returns>
        string UpgradeMessages();

        /// <summary>
        /// Manually trigger continue-update
        /// </summary>
        /// <returns></returns>
        bool ResumeAbortedUpgrade();

        string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
    }
}
