using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent.Environment.Dnn7
{
    internal partial class PagePublishing
    {


        private class ModuleVersionSettingsController
        {
            private const string LatestVersionSettingsKey = "LatestVersion";

            private const string PublishedVersionSettingsKey = "PublishedVersion";

            private readonly ModuleSettingsHelper _settingsHelper;


            public ModuleInfo ModuleInfo => _settingsHelper.ModuleInfo;


            public ModuleVersionSettingsController(int moduleId)
            {
                _settingsHelper = new ModuleSettingsHelper(moduleId);
            }


            public int GetPublishedVersion()
                => int.Parse(_settingsHelper.GetModuleSetting(PublishedVersionSettingsKey, "0"));

            public int GetLatestVersion()
                => int.Parse(_settingsHelper.GetModuleSetting(LatestVersionSettingsKey, "0"));

            public int IncreaseLatestVersion()
            {
                var version = GetLatestVersion() + 1;
                _settingsHelper.SetModuleSetting(LatestVersionSettingsKey, version.ToString());
                return version;
            }


            public void PublishLatestVersion()
                => _settingsHelper.SetModuleSetting(PublishedVersionSettingsKey, GetLatestVersion().ToString());

            public void DeleteLatestVersion()
                => _settingsHelper.SetModuleSetting(LatestVersionSettingsKey, GetPublishedVersion().ToString());

            public bool IsLatestVersionPublished() => GetLatestVersion() == GetPublishedVersion();
        }
    }

}