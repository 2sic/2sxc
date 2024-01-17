using DotNetNuke.Entities.Modules;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Dnn.Cms;

partial class DnnPagePublishing
{
    internal class ModuleVersions: HelperBase
    {
        private const string LatestVersionSettingsKey = "LatestVersion";

        private const string PublishedVersionSettingsKey = "PublishedVersion";

        private readonly ModuleSettingsHelper _settingsHelper;


        public ModuleInfo ModuleInfo => _settingsHelper.ModuleInfo;


        public ModuleVersions(int instanceId, ILog parentLog): base(parentLog, "Dnn.ModVer")
        {
            _settingsHelper = new(instanceId);
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
        {
            // 2017-09-13 must check maybe don't do anything, because 
            // this setting is already published by DNN when releasing the module
            _settingsHelper.SetModuleSetting(PublishedVersionSettingsKey, GetLatestVersion().ToString());
        }

        public void DeleteLatestVersion()
            => _settingsHelper.SetModuleSetting(LatestVersionSettingsKey, GetPublishedVersion().ToString());

        public bool IsLatestVersionPublished() => GetLatestVersion() == GetPublishedVersion();
    }
}