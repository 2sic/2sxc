using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class SettingsHelper
    {
        private readonly ISettingRepository _settingRepository;
        public Dictionary<string, string> Settings;

        public SettingsHelper(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public SettingsHelper Init(string entityName, int? id)
        {
            Settings = GetSettings(_settingRepository.GetSettings(entityName, id ?? -1).ToList());
            return this;
        }


        // Convert settings collection to Dictionary.
        private Dictionary<string, string> GetSettings(List<Setting> settings)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (Setting setting in settings.OrderBy(item => item.SettingName).ToList())
            {
                dictionary.Add(setting.SettingName, setting.SettingValue);
            }
            return dictionary;
        }
    }
}