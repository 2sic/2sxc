using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Integration;
using ToSic.Eav.Metadata;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Integration.Modules;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Dnn.Cms;

// TODO: @STV - this looks very similar to the Oqtane implementation
// Probably best to make a base class and de-duplicate.
internal class DnnModuleUpdater(
    GenWorkPlus<WorkViews> workViews,
    IZoneMapper zoneMapper,
    IAppsCatalog appsCatalog,
    ISite site)
    : ServiceBase("Dnn.MapA2I", connect: [workViews, appsCatalog, site, zoneMapper]), IPlatformModuleUpdater
{
    public void SetAppId(IModule instance, int? appId)
    {
        var l = Log.Fn($"SetAppIdForInstance({instance.Id}, -, appid: {appId})");
        // Reset temporary template
        ClearPreview(instance.Id);

        // ToDo: Should throw exception if a real BlockConfiguration exists
        // note: this is the correct zone, even if the module is shared from another portal, because the Site is prepared correctly
        var zoneId = site.ZoneId;

        if (appId is Eav.Constants.AppIdEmpty or null)
            UpdateInstanceSettingForAllLanguages(instance.Id, ModuleSettingNames.AppName, null, Log);
        else
        {
            var appName = appsCatalog.AppNameId(new AppIdentity(zoneId, appId.Value));
            UpdateInstanceSettingForAllLanguages(instance.Id, ModuleSettingNames.AppName, appName, Log);
        }

        // Change to 1. available preferable default template if app has been set
        if (appId.HasValue)
        {
            var appIdentity = new AppIdentity(zoneId, appId.Value);

            var templateGuid = workViews.New(appIdentity).GetAll()
                .OrderByDescending(v => v.Metadata.HasType(Decorators.IsDefaultDecorator)) // first sort by IsDefaultDecorator DESC
                .ThenBy(v => v.Name) // than by Name ASC
                .FirstOrDefault(t => !t.IsHidden)?.Guid;

            if (templateGuid.HasValue) SetPreview(instance.Id, templateGuid.Value);
        }

        l.Done();
    }

    protected void ClearPreview(int instanceId)
    {
        Log.A($"ClearPreviewTemplate(iid: {instanceId})");
        UpdateInstanceSettingForAllLanguages(instanceId, ModuleSettingNames.PreviewView, null, Log);
    }

    public void SetContentGroup(int instanceId, bool blockExists, Guid guid)
    {
        Log.A($"SetContentGroup(iid: {instanceId}, {nameof(blockExists)}: {blockExists}, guid: {guid})");
        // Remove Preview because it's not needed as soon Content is inserted
        ClearPreview(instanceId);
        // Update blockConfiguration Guid for this module
        if (blockExists)
            UpdateInstanceSettingForAllLanguages(instanceId, ModuleSettingNames.ContentGroup,
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
        if (settings[ModuleSettingNames.ContentGroup] != null)
            throw new("Preview template id cannot be set for a module that already has content.");

        UpdateInstanceSettingForAllLanguages(instanceId, ModuleSettingNames.PreviewView, previewView.ToString(), Log);
    }

    public void UpdateTitle(IBlock block, IEntity titleItem)
    {
        Log.A("update title");

        var languages = zoneMapper.CulturesWithState(block.Context.Site);

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