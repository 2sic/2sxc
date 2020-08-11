namespace ToSic.Sxc.Interfaces
{
    internal interface IEnvironmentInstaller
    {
        string UpgradeMessages();

        bool IsUpgradeRunning { get; }

        void ResumeAbortedUpgrade();
    }
}
