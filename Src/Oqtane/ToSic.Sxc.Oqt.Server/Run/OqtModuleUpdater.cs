using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Integration.Modules;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

// TODO: @STV - this looks very similar to the Dnn implementation
// Probably best to make a base class and de-duplicate.
internal class OqtModuleUpdater(
    SettingsHelper settingsHelper,
    IPageModuleRepository pageModuleRepository,
    GenWorkPlus<WorkViews> workViews,
    LazySvc<IAppsCatalog> appsCatalog,
    ISite site)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.MapA2I",
        connect: [settingsHelper, pageModuleRepository, appsCatalog, workViews, site]), IPlatformModuleUpdater
{
    public void SetAppId(IModule instance, int? appId)
    {
        var l = Log.Fn($"SetAppIdForInstance({instance.Id}, -, appid: {appId})");
        // Reset temporary template
        ClearPreview(instance.Id);

        // ToDo: Should throw exception if a real BlockConfiguration exists

        if (appId is Eav.Constants.AppIdEmpty or null)
            UpdateInstanceSetting(instance.Id, ModuleSettingNames.AppName, null, Log);
        else
        {
            var appName = appsCatalog.Value.AppNameId(new AppIdentity(site.ZoneId, appId.Value));
            UpdateInstanceSetting(instance.Id, ModuleSettingNames.AppName, appName, Log);
        }

        // Change to 1. available template if app has been set
        if (appId.HasValue)
        {
            var appIdentity = new AppIdentity(site.ZoneId, appId.Value);
            var templateGuid = workViews.New(appIdentity).GetAll()
                .FirstOrDefault(t => !t.IsHidden)?.Guid;
            if (templateGuid.HasValue) SetPreview(instance.Id, templateGuid.Value);
        }

        l.Done();
    }

    protected void ClearPreview(int instanceId)
    {
        Log.A($"ClearPreviewTemplate(iid: {instanceId})");
        UpdateInstanceSetting(instanceId, ModuleSettingNames.PreviewView, null, Log);
    }

    /// <summary>
    /// Update a setting for all language-versions of a module
    /// </summary>
    public void UpdateInstanceSetting(int instanceId, string key, string value, ILog log)
    {
        log.A($"UpdateInstanceSetting(iid: {instanceId}, key: {key}, val: {value})");

        if (value == null)
            settingsHelper.DeleteSetting(EntityNames.Module, instanceId, key);
        else
            settingsHelper.UpdateSetting(EntityNames.Module, instanceId, key, value);
    }

    /// <summary>
    /// Saves a temporary templateId to the module's settings
    /// This templateId will be used until a ContentGroup exists
    /// </summary>
    public void SetPreview(int instanceId, Guid previewTemplateGuid)
    {
        var settings = settingsHelper.Init(EntityNames.Module, instanceId).Settings;

        // Do not allow saving the temporary template id if a ContentGroup exists for this module
        if (settings.TryGetValue(ModuleSettingNames.ContentGroup, out var value) && !string.IsNullOrEmpty(value))
            throw new("Preview template id cannot be set for a module that already has content.");

        UpdateInstanceSetting(instanceId, ModuleSettingNames.PreviewView, previewTemplateGuid.ToString(), Log);
    }
    public void SetContentGroup(int instanceId, bool wasCreated, Guid guid)
    {
        Log.A($"SetContentGroup(iid: {instanceId}, {nameof(wasCreated)}: {wasCreated}, guid: {guid})");
        // Remove Preview because it's not needed as soon Content is inserted
        ClearPreview(instanceId);
        // Update blockConfiguration Guid for this module
        if (wasCreated)
            UpdateInstanceSetting(instanceId, ModuleSettingNames.ContentGroup, guid.ToString(), Log);
    }

    public void UpdateTitle(IBlock block, IEntity titleItem)
    {
        Log.A("update title");

        // Module tile is stored in PageModule, so we need moduleId and pageId to update it.
        var pageId = block.Context.Page.Id;
        var moduleId = block.Context.Module.Id;
        var pageModule = pageModuleRepository.GetPageModule(pageId, moduleId);
        pageModule.Title = System.Net.WebUtility.HtmlEncode(titleItem.GetBestTitle());
        pageModuleRepository.UpdatePageModule(pageModule);
    }
}