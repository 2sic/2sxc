using System;
using Oqtane.Models;
using Oqtane.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.App
{
    public partial class Settings
    {
        public override string Title => "Sxc Settings";

        private string _value;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Dictionary<string, string> settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _value = SettingService.GetSetting(settings, "SettingName", "");
            }
            catch (Exception ex)
            {
                ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error);
            }
        }

        public async Task UpdateSettings()
        {
            try
            {
                Dictionary<string, string> settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                SettingService.SetSetting(settings, "SettingName", _value);
                await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
            }
            catch (Exception ex)
            {
                ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error);
            }
        }
    }
}
