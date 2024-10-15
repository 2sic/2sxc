using System;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Integration;

internal class SettingsHelper(ISettingRepository settingRepository)
{
    public Dictionary<string, string> Settings;

    public SettingsHelper Init(string entityName, int? id)
    {
        Settings = GetSettings(settingRepository.GetSettings(entityName, id ?? -1).ToList());
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
        return settingRepository
            .GetSettings(entityName, entityId)
            .FirstOrDefault(item => item.SettingName == settingName);
    }

    public void DeleteSetting(string entityName, int entityId, string settingName)
    {
        var delete = settingRepository
            .GetSettings(entityName, entityId)
            .FirstOrDefault(item => item.SettingName == settingName);

        if (delete != null)
        {
            //_settingRepository.DeleteSetting(settingId: delete.SettingId); // can't use in Oqt 3.0.1+ because of change in signature
            // workaround code that works in Oqt 2.3.1+
            delete.SettingValue = string.Empty;
            delete.ModifiedOn = DateTime.Now;
            delete.ModifiedBy = WipConstants.SettingsChangeUserId;
            settingRepository.UpdateSetting(delete);
        }
    }

    public void UpdateSetting(string entityName, int entityId, string settingName, string settingValue)
    {
        var update = settingRepository
            .GetSettings(entityName, entityId)
            .FirstOrDefault(item => item.SettingName == settingName);

        if (update != null)
        {
            update.SettingValue = settingValue;
            update.ModifiedOn = DateTime.Now;
            update.ModifiedBy = WipConstants.SettingsChangeUserId;
            settingRepository.UpdateSetting(update);
        }
        else
            settingRepository.AddSetting(new()
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