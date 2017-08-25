using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class ModuleSettingsController // : ISettingsController
    {
        public ModuleInfo ModuleInfo { get; set; }

        public ModuleSettingsController(int moduleId)
        {
            ModuleInfo = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
        }

        public string GetModuleSetting(string settingName, string defaultValue)
        {
            if (ModuleInfo.ModuleSettings.ContainsKey(settingName))
            {
                return (string)ModuleInfo.ModuleSettings[settingName];
            }
            return defaultValue;
        }

        public void SetModuleSetting(string settingName, string settingValue)
        {
            if (ModuleInfo.ModuleSettings.ContainsKey(settingName))
            {
                ModuleInfo.ModuleSettings[settingName] = settingValue;
            }
            else
            {
                ModuleInfo.ModuleSettings.Add(settingName, settingValue);
            }
            ModuleController.Instance.UpdateModule(ModuleInfo);
        }

        public void DeleteModuleSetting(string settingName)
        {
            if (ModuleInfo.ModuleSettings.ContainsKey(settingName))
            {
                ModuleInfo.ModuleSettings.Remove(settingName);
                ModuleController.Instance.UpdateModule(ModuleInfo);
            }
        }
    }
}
