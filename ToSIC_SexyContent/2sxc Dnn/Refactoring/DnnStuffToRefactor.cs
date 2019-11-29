using System.IO;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;

namespace ToSic.SexyContent.Internal
{
    public class DnnStuffToRefactor
    {
        // todo: try to cache the result of settings-stored in a static variable, this full check
        // todo: shouldn't have to happen every time

        /// <summary>
        /// Returns true if the Portal HomeDirectory Contains the 2sxc Folder and this folder contains the web.config and a Content folder
        /// </summary>
        public void EnsureTenantIsConfigured(ICmsBlock cms, HttpServerUtility server, string controlPath)
        {
            var sexyFolder = new DirectoryInfo(server.MapPath(cms.Block.Tenant.SxcPath));
            var contentFolder = new DirectoryInfo(Path.Combine(sexyFolder.FullName, Constants.ContentAppName));
            var webConfigTemplate = new FileInfo(Path.Combine(sexyFolder.FullName, Settings.WebConfigFileName));
            if (!(sexyFolder.Exists && webConfigTemplate.Exists && contentFolder.Exists))
            {
                // configure it
                var tm = new TemplateHelpers(cms.App);
                tm.EnsureTemplateFolderExists(Settings.TemplateLocations.PortalFileSystem);
            };
        }


        #region Settings

        

        
        /// <summary>
        /// Update a setting for all language-versions of a module
        /// </summary>
        public static void UpdateInstanceSettingForAllLanguages(int instanceId, string key, string value, ILog log)
        {
            log?.Add($"UpdateInstanceSettingForAllLanguages(iid: {instanceId}, key: {key}, val: {value})");
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