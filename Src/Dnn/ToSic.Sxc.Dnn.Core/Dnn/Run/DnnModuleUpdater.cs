using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;


namespace ToSic.Sxc.Dnn.Run
{
    public class DnnModuleUpdater : HasLog, IPlatformModuleUpdater
    {
        #region Constructor and DI

        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DnnModuleUpdater(Lazy<CmsRuntime> cmsRuntimeLazy, IZoneMapper zoneMapper, IAppStates appStates, ISite site) : base("Dnn.MapA2I")
        {
            _cmsRuntimeLazy = cmsRuntimeLazy;
            _appStates = appStates;
            _site = site;
            _zoneMapper = zoneMapper.Init(Log);
        }
        private readonly Lazy<CmsRuntime> _cmsRuntimeLazy;
        private readonly IAppStates _appStates;
        private readonly ISite _site;
        private readonly IZoneMapper _zoneMapper;


        #endregion


        public void SetAppId(IModule instance, int? appId)
        {
            Log.A($"SetAppIdForInstance({instance.Id}, -, appid: {appId})");
            // Reset temporary template
            ClearPreview(instance.Id);

            // ToDo: Should throw exception if a real BlockConfiguration exists
            // note: this is the correct zone, even if the module is shared from another portal, because the Site is prepared correctly
            var zoneId = _site.ZoneId;

            if (appId == Eav.Constants.AppIdEmpty || !appId.HasValue)
                UpdateInstanceSettingForAllLanguages(instance.Id, Settings.ModuleSettingApp, null, Log);
            else
            {
                var appName = _appStates.AppIdentifier(zoneId, appId.Value);
                UpdateInstanceSettingForAllLanguages(instance.Id, Settings.ModuleSettingApp, appName, Log);
            }

            // Change to 1. available preferable default template if app has been set
            if (appId.HasValue)
            {
                var appIdentity = new AppIdentity(zoneId, appId.Value);
                var cms = _cmsRuntimeLazy.Value.Init(Log).InitQ(appIdentity, true);

                var templateGuid = cms.Views.GetAll()
                    .OrderByDescending(v => v.Metadata.HasType(Decorators.IsDefaultDecorator)) // first sort by IsDefaultDecorator DESC
                    .ThenBy(v => v.Name) // than by Name ASC
                    .FirstOrDefault(t => !t.IsHidden)?.Guid;

                if (templateGuid.HasValue) SetPreview(instance.Id, templateGuid.Value);
            }
        }

        protected void ClearPreview(int instanceId)
        {
            Log.A($"ClearPreviewTemplate(iid: {instanceId})");
            UpdateInstanceSettingForAllLanguages(instanceId, Settings.ModuleSettingsPreview, null, Log);
        }

        public void SetContentGroup(int instanceId, bool blockExists, Guid guid)
        {
            Log.A($"SetContentGroup(iid: {instanceId}, {nameof(blockExists)}: {blockExists}, guid: {guid})");
            // Remove Preview because it's not needed as soon Content is inserted
            ClearPreview(instanceId);
            // Update blockConfiguration Guid for this module
            if (blockExists)
                UpdateInstanceSettingForAllLanguages(instanceId, Settings.ModuleSettingContentGroup,
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
            if (settings[Settings.ModuleSettingContentGroup] != null)
                throw new Exception("Preview template id cannot be set for a module that already has content.");

            UpdateInstanceSettingForAllLanguages(instanceId, Settings.ModuleSettingsPreview, previewView.ToString(), Log);
        }

        public void UpdateTitle(IBlock block, IEntity titleItem)
        {
            Log.A("update title");

            var languages = _zoneMapper.CulturesWithState(block.Context.Site);

            // Find Module for default language
            var moduleController = new ModuleController();
            var originalModule = moduleController.GetModule(block.Context.Module.Id);

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
                    var titleValue = titleItem.Title[dimension.Code].ToString();

                    // Find module for given Culture
                    var moduleByCulture = moduleController.GetModuleByCulture(originalModule.ModuleID,
                        originalModule.TabID, block.Context.Site.Id,
                        DotNetNuke.Services.Localization.LocaleController.Instance.GetLocale(dimension.Code));

                    // Break if no title module found
                    if (moduleByCulture == null || titleValue == null)
                        return;

                    moduleByCulture.ModuleTitle = System.Net.WebUtility.HtmlEncode(titleValue);
                    moduleController.UpdateModule(moduleByCulture);
                }
                catch { /* ignored */ }
            }
        }


        #region Settings




        /// <summary>
        /// Update a setting for all language-versions of a module
        /// </summary>
        public static void UpdateInstanceSettingForAllLanguages(int instanceId, string key, string value, ILog log)
        {
            log.A($"UpdateInstanceSettingForAllLanguages(iid: {instanceId}, key: {key}, val: {value})");
            var moduleController = new ModuleController();

            // Find this module in other languages and update contentGroupGuid
            var originalModule = moduleController.GetModule(instanceId);
            var languages = LocaleController.Instance.GetLocales(originalModule.PortalID);

            if (!originalModule.IsDefaultLanguage && originalModule.DefaultLanguageModule != null)
                originalModule = originalModule.DefaultLanguageModule;

            foreach (var language in languages)
            {
                // Find module for given Culture
                var moduleByCulture = moduleController.GetModuleByCulture(originalModule.ModuleID, originalModule.TabID, originalModule.PortalID, language.Value);

                // Break if no module found
                if (moduleByCulture == null)
                    continue;

                if (value == null)
                    moduleController.DeleteModuleSetting(moduleByCulture.ModuleID, key);
                else
                    moduleController.UpdateModuleSetting(moduleByCulture.ModuleID, key, value);
            }
        }
        #endregion
    }
}