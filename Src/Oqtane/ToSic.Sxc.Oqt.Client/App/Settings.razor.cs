using System;
using Oqtane.Models;
using Oqtane.UI;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Oqtane.Modules;
using SxcSettings = ToSic.Sxc.Settings;

namespace ToSic.Sxc.Oqt.App
{
    public partial class Settings
    {
        public override string Title => "2sxc App Settings";

        //private string _eavApp;
        //private string _eavContentGroup;
        //private string _eavPreview;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //Dictionary<string, string> settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);

                //_eavApp = SettingService.GetSetting(settings, SxcSettings.ModuleSettingApp, "");
                //_eavContentGroup = SettingService.GetSetting(settings, SxcSettings.ModuleSettingContentGroup, "");
                //_eavPreview = SettingService.GetSetting(settings, SxcSettings.ModuleSettingsPreview, "");
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
                //Dictionary<string, string> settings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);

                //SettingService.SetSetting(settings, SxcSettings.ModuleSettingApp, _eavApp);
                //SettingService.SetSetting(settings, SxcSettings.ModuleSettingContentGroup, _eavContentGroup);
                //SettingService.SetSetting(settings, SxcSettings.ModuleSettingsPreview, _eavPreview);

                //await SettingService.UpdateModuleSettingsAsync(settings, ModuleState.ModuleId);
            }
            catch (Exception ex)
            {
                ModuleInstance.AddModuleMessage(ex.Message, MessageType.Error);
            }
        }
    }
}
