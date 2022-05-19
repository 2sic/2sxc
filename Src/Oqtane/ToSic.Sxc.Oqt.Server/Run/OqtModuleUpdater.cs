using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    internal class OqtModuleUpdater: HasLog<IPlatformModuleUpdater>, IPlatformModuleUpdater
    {
        private readonly SettingsHelper _settingsHelper;
        private readonly IPageModuleRepository _pageModuleRepository;
        private readonly Lazy<CmsRuntime> _lazyCmsRuntime;
        private readonly IAppStates _appStates;
        private readonly ISite _site;

        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public OqtModuleUpdater(SettingsHelper settingsHelper,
            IPageModuleRepository pageModuleRepository,
            Lazy<CmsRuntime> lazyCmsRuntime, IAppStates appStates, ISite site) : base($"{OqtConstants.OqtLogPrefix}.MapA2I")
        {
            _settingsHelper = settingsHelper;
            _pageModuleRepository = pageModuleRepository;
            _lazyCmsRuntime = lazyCmsRuntime;
            _appStates = appStates;
            _site = site;
        }

        public void SetAppId(IModule instance, int? appId)
        {
            Log.A($"SetAppIdForInstance({instance.Id}, -, appid: {appId})");
            // Reset temporary template
            ClearPreview(instance.Id);

            // ToDo: Should throw exception if a real BlockConfiguration exists

            if (appId == Eav.Constants.AppIdEmpty || !appId.HasValue)
                UpdateInstanceSetting(instance.Id, Settings.ModuleSettingApp, null, Log);
            else
            {
                var appName = _appStates.AppIdentifier(_site.ZoneId, appId.Value);
                UpdateInstanceSetting(instance.Id, Settings.ModuleSettingApp, appName, Log);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var appIdentity = new AppIdentity(_site.ZoneId, appId.Value);
                var cms = _lazyCmsRuntime.Value.Init(appIdentity, true, Log);
                var templateGuid = cms.Views.GetAll().FirstOrDefault(t => !t.IsHidden)?.Guid;
                if (templateGuid.HasValue) SetPreview(instance.Id, templateGuid.Value);
            }
        }

        protected void ClearPreview(int instanceId)
        {
            Log.A($"ClearPreviewTemplate(iid: {instanceId})");
            UpdateInstanceSetting(instanceId, Settings.ModuleSettingsPreview, null, Log);
        }

        /// <summary>
        /// Update a setting for all language-versions of a module
        /// </summary>
        public void UpdateInstanceSetting(int instanceId, string key, string value, ILog log)
        {
            log.A($"UpdateInstanceSetting(iid: {instanceId}, key: {key}, val: {value})");

            if (value == null)
                _settingsHelper.DeleteSetting(EntityNames.Module, instanceId, key);
            else
                _settingsHelper.UpdateSetting(EntityNames.Module, instanceId, key, value);
        }

        /// <summary>
        /// Saves a temporary templateId to the module's settings
        /// This templateId will be used until a ContentGroup exists
        /// </summary>
        public void SetPreview(int instanceId, Guid previewTemplateGuid)
        {
            var settings = _settingsHelper.Init(EntityNames.Module, instanceId).Settings;

            // Do not allow saving the temporary template id if a ContentGroup exists for this module
            if (settings.TryGetValue(Settings.ModuleSettingContentGroup, out var value) && !string.IsNullOrEmpty(value))
                throw new Exception("Preview template id cannot be set for a module that already has content.");

            UpdateInstanceSetting(instanceId, Settings.ModuleSettingsPreview, previewTemplateGuid.ToString(), Log);
        }
        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid)
        {
            Log.A($"SetContentGroup(iid: {instanceId}, {nameof(wasCreated)}: {wasCreated}, guid: {guid})");
            // Remove Preview because it's not needed as soon Content is inserted
            ClearPreview(instanceId);
            // Update blockConfiguration Guid for this module
            if (wasCreated)
                UpdateInstanceSetting(instanceId, Settings.ModuleSettingContentGroup, guid.ToString(), Log);
        }

        public void UpdateTitle(IBlock block, IEntity titleItem)
        {
            Log.A("update title");

            // Module tile is stored in PageModule, so we need moduleId and pageId to update it.
            var pageId = block.Context.Page.Id;
            var moduleId = block.Context.Module.Id;
            var pageModule = _pageModuleRepository.GetPageModule(pageId, moduleId);
            pageModule.Title = System.Net.WebUtility.HtmlEncode(titleItem.GetBestTitle());
            _pageModuleRepository.UpdatePageModule(pageModule);
        }
    }
}
