namespace ToSic.SexyContent.Interfaces
{
    public interface IInstallerEnvironment
    {
        string UpgradeMessages();

        bool IsUpgradeRunning { get; }

        void ResumeAbortedUpgrade();
    }
}
