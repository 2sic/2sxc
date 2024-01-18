namespace ToSic.Sxc.Integration.Installation;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IEnvironmentInstaller: IHasLog
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

    //string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp);
}