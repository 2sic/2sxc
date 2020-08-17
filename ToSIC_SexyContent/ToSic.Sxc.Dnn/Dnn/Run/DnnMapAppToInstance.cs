using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnMapAppToInstance : HasLog, IEnvironmentConnector
    {
        #region Constructor and DI

        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DnnMapAppToInstance() : base("Dnn.MapA2I") { }

        public DnnMapAppToInstance(ILog parentLog) : base("Dnn.MapA2I", parentLog) { }

        public IEnvironmentConnector Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        #endregion


        public void SetAppId(IContainer instance, IAppEnvironment env, int? appId, ILog parentLog)
        {
            Log.Add($"SetAppIdForInstance({instance.Id}, -, appid: {appId})");
            // Reset temporary template
            ClearPreview(instance.Id);

            // ToDo: Should throw exception if a real BlockConfiguration exists

            var module = (instance as Container<ModuleInfo>).UnwrappedContents;
            var zoneId = env.ZoneMapper.GetZoneId(module.OwnerPortalID);

            if (appId == Constants.AppIdEmpty || !appId.HasValue)
                DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instance.Id, Settings.AppNameString, null, Log);
            else
            {
                var appName = State.Zones[zoneId].Apps[appId.Value];
                DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instance.Id, Settings.AppNameString, appName, Log);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var appIdentity = new AppIdentity(zoneId, appId.Value);
                var cms = new CmsRuntime(appIdentity, Log, true, env.PagePublishing.IsEnabled(instance.Id));
                var templateGuid = cms.Views.GetAll().FirstOrDefault(t => !t.IsHidden)?.Guid;
                if (templateGuid.HasValue) SetPreview(instance.Id, templateGuid.Value);
            }
        }

        protected void ClearPreview(int instanceId)
        {
            Log.Add($"ClearPreviewTemplate(iid: {instanceId})");
            DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instanceId, Settings.FieldPreviewTemplate, null, Log);
        }

        public void SetContentGroup(int instanceId, bool blockExists, Guid guid)
        {
            Log.Add($"SetContentGroup(iid: {instanceId}, {nameof(blockExists)}: {blockExists}, guid: {guid})");
            // Remove Preview because it's not needed as soon Content is inserted
            ClearPreview(instanceId);
            // Update blockConfiguration Guid for this module
            if (blockExists)
                DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instanceId, Settings.FieldContentGroup,
                    guid.ToString(), Log);
        }

        /// <summary>
        /// Saves a temporary templateId to the module's settings
        /// This templateId will be used until a ContentGroup exists
        /// </summary>
        public void SetPreview(int instanceId, Guid previewView)
        {
            var moduleController = new ModuleController();
            var settings = moduleController.GetModule(instanceId).ModuleSettings;

            // Do not allow saving the temporary template id if a ContentGroup exists for this module
            if (settings[Settings.FieldContentGroup] != null)
                throw new Exception("Preview template id cannot be set for a module that already has content.");

            DnnTenantSettings.UpdateInstanceSettingForAllLanguages(instanceId, Settings.FieldPreviewTemplate, previewView.ToString(), Log);
        }

        public void UpdateTitle(Blocks.IBlockBuilder blockBuilder, IEntity titleItem)
        {
            Log.Add("update title");

            var languages = blockBuilder.Environment.ZoneMapper.CulturesWithState(blockBuilder.Context.Tenant.Id,
                blockBuilder.Block.ZoneId);

            // Find Module for default language
            var moduleController = new ModuleController();
            var originalModule = moduleController.GetModule(blockBuilder.Context.Container.Id);

            foreach (var dimension in languages)
            {
                if (!originalModule.IsDefaultLanguage)
                    originalModule = originalModule.DefaultLanguageModule;

                try // this can sometimes fail, like if the first item is null - https://github.com/2sic/2sxc/issues/817
                {
                    // Break if default language module is null
                    if (originalModule == null)
                        return;

                    // Get Title value of Entity in current language
                    var titleValue = titleItem.Title[dimension.Key].ToString();

                    // Find module for given Culture
                    var moduleByCulture = moduleController.GetModuleByCulture(originalModule.ModuleID,
                        originalModule.TabID, blockBuilder.Context.Tenant.Id,
                        DotNetNuke.Services.Localization.LocaleController.Instance.GetLocale(dimension.Key));

                    // Break if no title module found
                    if (moduleByCulture == null || titleValue == null)
                        return;

                    moduleByCulture.ModuleTitle = titleValue;
                    moduleController.UpdateModule(moduleByCulture);
                }
                catch { /* ignored */ }
            }
        }

    }
}