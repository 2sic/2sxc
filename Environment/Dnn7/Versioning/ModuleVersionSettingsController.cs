using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ModuleVersionSettingsController
    {
        private const string LatestVersionSettingsKey = "LatestVersion";

        private const string PublishedVersionSettingsKey = "PublishedVersion";

        private ModuleSettingsController settingsController;


        public ModuleInfo ModuleInfo { get { return settingsController.ModuleInfo; } }


        public ModuleVersionSettingsController(int moduleId)
        {
            settingsController = new ModuleSettingsController(moduleId);
        }


        public int GetPublishedVersion()
        {
            return int.Parse(settingsController.GetModuleSetting(PublishedVersionSettingsKey, "0"));
        }

        public int GetLatestVersion()
        {
            return int.Parse(settingsController.GetModuleSetting(LatestVersionSettingsKey, "0"));
        }

        public int IncreaseLatestVersion()
        {
            var version = GetLatestVersion() + 1;
            settingsController.SetModuleSetting(LatestVersionSettingsKey, version.ToString());
            return version;
        }


        public void PublishLatestVersion()
        {
            settingsController.SetModuleSetting(PublishedVersionSettingsKey, GetLatestVersion().ToString());
        }

        public void DeleteLatestVersion()
        {
            settingsController.SetModuleSetting(LatestVersionSettingsKey, GetPublishedVersion().ToString());
        }

        public bool IsLatestVersionPublished()
        {
            return GetLatestVersion() == GetPublishedVersion();
        }
    }
}
