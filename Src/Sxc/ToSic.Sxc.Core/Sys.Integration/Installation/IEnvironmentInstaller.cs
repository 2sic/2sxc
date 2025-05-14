namespace ToSic.Sxc.Integration.Installation;

[ShowApiWhenReleased(ShowApiMode.Never)]
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
}