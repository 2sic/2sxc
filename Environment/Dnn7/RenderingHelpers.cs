using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Environment.Dnn7
{
    internal class RenderingHelpers
    {
        private SxcInstance _sxcInstance;
        private PortalSettings PortalSettings;
        private int TabId;
        private int ModuleId;
        private UserInfo UserInfo;
        private string ApplicationRoot;
        private ModuleInstanceContext ModuleContext;

        private int? AppId;
        private bool _usingStoredConfig;

        internal RenderingHelpers(SxcInstance sxc, ModuleInstanceContext mic, bool hasStoredConfiguration, string appRoot)
        {
            _sxcInstance = sxc;
            PortalSettings = mic.PortalSettings;// PortalSettings.Current;
            TabId = mic.TabId;// tabId;
            ModuleId = mic.ModuleId;// modId;
            UserInfo = PortalSettings.Current.UserInfo;
            ApplicationRoot = appRoot;
            ModuleContext = mic;
            _usingStoredConfig = hasStoredConfiguration;
            AppId = hasStoredConfiguration ? sxc.AppId : null;


        }

        internal object InfosForTheClientScripts()
        {
            var hasContent = _usingStoredConfig && _sxcInstance.Template != null && _sxcInstance.ContentGroup.Exists;

            // minor workaround because the settings in the cache are wrong after using a page template
            var tempVisibleStatus = DnnStuffToRefactor.TryToGetReliableSetting(_sxcInstance.ModuleInfo,
                Settings.SettingsShowTemplateChooser);
            var templateChooserVisible = bool.Parse(tempVisibleStatus ?? "true");

            var languages =
                ZoneHelpers.GetCulturesWithActiveState(PortalSettings.PortalId, _sxcInstance.ZoneId.Value)
                    .Where(c => c.Active)
                    .Select(c => new { key = c.Code.ToLower(), name = c.Text });

            var priLang = PortalSettings.DefaultLanguage.ToLower(); // primary language 

            // for now, don't filter by existing languages, this causes side-effects in many cases. 
            //if (!languages.Where(l => l.key == priLang).Any())
            //    priLang = "";

            var clientInfos = new
            {
                moduleId = ModuleId,
                manage = new
                {
                    isEditMode = _sxcInstance?.Environment?.Permissions.UserMayEditContent ?? false,
                    templateChooserVisible,
                    hasContent,
                    isContentApp = _sxcInstance.IsContentApp,
                    zoneId = _sxcInstance.ZoneId ?? 0,
                    appId = _usingStoredConfig ? _sxcInstance.AppId : null,
                    isList = _usingStoredConfig && _sxcInstance.ContentGroup.Content.Count > 1,
                    templateId = _sxcInstance.Template?.TemplateId,
                    contentTypeId = _sxcInstance.Template?.ContentTypeStaticName ?? "",
                    config = new
                    {
                        portalId = PortalSettings.PortalId,
                        tabId =  TabId,
                        moduleId = ModuleId,
                        contentGroupId = _usingStoredConfig  ? _sxcInstance.ContentGroup.ContentGroupGuid : (Guid?)null,
                        dialogUrl = Globals.NavigateURL(TabId),
                        // 2016-03-01 2dm - probably unused now returnUrl = Request.RawUrl,
                        appPath = _usingStoredConfig ? _sxcInstance.App.Path + "/" : null,
                        // 2016-02-27 2dm - seems unused
                        //cultureDimension = AppId.HasValue ? Sexy.GetCurrentLanguageID() : new int?(),
                        isList = _sxcInstance.Template?.UseForList ?? false,
                        version = Settings.Version.ToString() // SexyContent.Version.ToString()
                    },
                    user = new
                    {
                        canDesign = SecurityHelpers.IsInSexyContentDesignersGroup(UserInfo),
                        // will be true for admins or for people in the designers-group
                        canDevelop = UserInfo.IsSuperUser // will be true for host-users, false for all others
                    },
                    applicationRoot = ApplicationRoot,// ResolveUrl("~"),
                    lang = PortalSettings.CultureCode.ToLower(),
                    //System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower(),
                    langPrimary = priLang,
                    languages
                }
            };
            return clientInfos;
        }


        internal void RegisterClientDependencies(Page Page, bool useDebug)
        {
            var root = "~/desktopmodules/tosic_sexycontent/";
            var ext = /*string.IsNullOrEmpty( Request.QueryString["debug"])*/ useDebug? ".min.js" : ".js";

            // add edit-mode CSS
            ClientResourceManager.RegisterStyleSheet(Page, root + "dist/inpage/inpage.min.css");

            // ToDo: Move these RegisterScripts to JS to prevent including AngularJS twice (from other modules)
            ClientResourceManager.RegisterScript(Page, root + "js/angularjs/angular.min.js", 80);

            // New: multi-language stuff
            ClientResourceManager.RegisterScript(Page, root + "dist/lib/i18n/set.min.js");

            ClientResourceManager.RegisterScript(Page, root + "js/2sxc.api" + ext, 90);
            ClientResourceManager.RegisterScript(Page, root + "dist/inpage/inpage" + ext, 91);

            ClientResourceManager.RegisterScript(Page, root + "js/angularjs/2sxc4ng" + ext, 93);
            ClientResourceManager.RegisterScript(Page, root + "dist/config/config" + ext, 93);
        }

        /// <summary>
        /// Add data-2sxc-globals Attribute to the DNN ModuleHost
        /// </summary>
        internal object RegisterGlobalsAttribute()
        {
            var globData = new
            {
                ModuleContext = new
                {
                    ModuleContext.PortalId,
                    ModuleContext.TabId,
                    ModuleContext.ModuleId,
                    AppId = _usingStoredConfig ? 
                    _sxcInstance?.App?.AppId : null// AppId
                },
                PortalSettings.ActiveTab.FullUrl,
                //PortalRoot = (Request.IsSecureConnection ? "https://" : "http://") + PortalAlias.HTTPAlias + "/",
                PortalRoot = "//" + PortalSettings.PortalAlias.HTTPAlias + "/",
                DefaultLanguageID = _sxcInstance?.EavAppContext.Dimensions.GetLanguageId(PortalSettings.DefaultLanguage)
            };
            return globData;
        }

    }
}