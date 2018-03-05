namespace ToSic.SexyContent.Interfaces
{
    public interface IEnvironmentInstaller
    {
        string UpgradeMessages();

        bool IsUpgradeRunning { get; }

        void ResumeAbortedUpgrade();
    }
}
