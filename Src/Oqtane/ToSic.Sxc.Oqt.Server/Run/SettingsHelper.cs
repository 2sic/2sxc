﻿using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

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
            return settings.OrderBy(item => item.SettingName)
                .ToList()
                .ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);
        }

        public Setting GetSetting(string entityName, int entityId, string settingName)
        {
            return _settingRepository
                .GetSettings(entityName, entityId)
                .FirstOrDefault(item => item.SettingName == settingName);
        }

        public void DeleteSetting(string entityName, int entityId, string settingName)
        {
            var delete = _settingRepository
                .GetSettings(entityName, entityId)
                .FirstOrDefault(item => item.SettingName == settingName);

            if (delete != null) _settingRepository.DeleteSetting(delete.SettingId);
        }

        public void UpdateSetting(string entityName, int entityId, string settingName, string settingValue)
        {
            var update = _settingRepository
                .GetSettings(entityName, entityId)
                .FirstOrDefault(item => item.SettingName == settingName);

            if (update != null)
            {
                update.SettingValue = settingValue;
                update.ModifiedOn = DateTime.Now;
                update.ModifiedBy = WipConstants.SettingsChangeUserId;
                _settingRepository.UpdateSetting(update);
            }
            else
                _settingRepository.AddSetting(new Setting
                {
                    CreatedBy = WipConstants.SettingsChangeUserId,
                    CreatedOn = DateTime.Now,
                    EntityName = EntityNames.Module,
                    EntityId = entityId, 
                    ModifiedOn = DateTime.Now, 
                    ModifiedBy = WipConstants.SettingsChangeUserId,
                    SettingName = settingName, 
                    SettingValue = settingValue
                });
        }
    }
}