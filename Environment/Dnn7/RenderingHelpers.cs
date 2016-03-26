using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                moduleId = ModuleId, // inst
                manage = new
                {
                    isEditMode = _sxcInstance?.Environment?.Permissions.UserMayEditContent ?? false, // itm level
                    templateChooserVisible, // cblock... ok
                    hasContent, // cb ok
                    isContentApp = _sxcInstance.IsContentApp, // cb
                    zoneId = _sxcInstance.ZoneId ?? 0, // cb / inst ok
                    appId = _usingStoredConfig ? _sxcInstance.AppId : null, // cb ok
                    isList = _usingStoredConfig && _sxcInstance.ContentGroup.Content.Count > 1, // cb ok
                    templateId = _sxcInstance.Template?.TemplateId, // cb ok
                    contentTypeId = _sxcInstance.Template?.ContentTypeStaticName ?? "", // cb / itm ok
                    config = new
                    {
                        portalId = PortalSettings.PortalId, // inst ok
                        tabId =  TabId, // inst ok
                        moduleId = ModuleId, // inst ok
                        contentGroupId = _usingStoredConfig  ? _sxcInstance.ContentGroup.ContentGroupGuid : (Guid?)null, // cb ok
                        dialogUrl = Globals.NavigateURL(TabId), // syst unused?
                        appPath = _usingStoredConfig ? _sxcInstance.App.Path + "/" : null, // cb ok
                        isList = _sxcInstance.Template?.UseForList ?? false, // cb ok
                        version = Settings.Version.ToString()  // syst ok
                    },
                    user = new // ok
                    {
                        canDesign = SecurityHelpers.IsInSexyContentDesignersGroup(UserInfo), // syst / inst / cb ok
                        // will be true for admins or for people in the designers-group
                        canDevelop = UserInfo.IsSuperUser // will be true for host-users, false for all others // syst / inst ok
                    },
                    applicationRoot = ApplicationRoot,// cb ok
                    lang = PortalSettings.CultureCode.ToLower(), // syst ok
                    langPrimary = priLang, // syst ok
                    languages // syst ok
                }
            };
            return clientInfos;
        }


        internal void RegisterClientDependencies(Page Page, bool useDebug)
        {
            var root = "~/desktopmodules/tosic_sexycontent/";
            root = Page.ResolveUrl(root);
            var breakCache = "?sxcver=" + Settings.Version;
            var ext = (useDebug? ".min.js" : ".js" + breakCache);
            var ver = Settings.Version.ToString();

            // add edit-mode CSS
            RegisterCss(Page, Settings.Version.ToString(), root + "dist/inpage/inpage.min.css");

            // ToDo: Move these RegisterScripts to JS to prevent including AngularJS twice (from other modules)
            // ClientResourceManager.RegisterScript(Page, root + "js/angularjs/angular.min.js" + breakCache, 80);
            //RegisterJs(Page, ver, root + "js/angularjs/angular.min.js");

            // New: multi-language stuff
            //ClientResourceManager.RegisterScript(Page, root + "dist/lib/i18n/set.min.js" + breakCache);
            //RegisterJs(Page, ver, root + "dist/lib/i18n/set.min.js");

            //ClientResourceManager.RegisterScript(Page, root + "js/2sxc.api" + ext, 90);
            //ClientResourceManager.RegisterScript(Page, root + "dist/inpage/inpage" + ext, 91);
            RegisterJs(Page, ver, root + "js/2sxc.api" + ext);
            RegisterJs(Page, ver, root + "dist/inpage/inpage" + ext);
            //RegisterJs(Page, ver, root + "dist/inpage/inpage-dialogs" + ext);

            //ClientResourceManager.RegisterScript(Page, root + "js/angularjs/2sxc4ng" + ext, 93);
            //ClientResourceManager.RegisterScript(Page, root + "dist/config/config" + ext, 93);
            //RegisterJs(Page, ver, root + "js/angularjs/2sxc4ng" + ext);
            //RegisterJs(Page, ver, root + "dist/config/config" + ext);

        }

        #region add scripts / css with bypassing the official ClientResourceManager

        public static void RegisterJs(Page page, string version, string path)
        {
            var url = string.Format("{0}{1}v={2}", path, path.IndexOf('?') > 0 ? '&' : '?', version);
            page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
        }



        public static void RegisterCss(Page page, string version, string path)
        {
            ClientResourceManager.RegisterStyleSheet(page, path);

            // alternative, but will add the same tag many times, don't use yet...
            //var include = new Literal();
            //include.Text = string.Format("<link href =\"{0}{1}v={2}\" type=\"text/css\" rel=\"stylesheet\" />", 
            //    path, path.IndexOf('?') > 0 ? '&' : '?', version);
            //page.Header.Controls.Add(include);
        }


        #endregion

        /// <summary>
        /// Add data-2sxc-globals Attribute to the DNN ModuleHost
        /// </summary>
        internal object RegisterGlobalsAttribute()
        {
            var globData = new
            {
                ModuleContext = new
                {
                    ModuleContext.PortalId, // syst / inst - ok
                    ModuleContext.TabId,  // syst / inst - ok
                    ModuleContext.ModuleId, // inst
                    AppId = _usingStoredConfig ? _sxcInstance?.App?.AppId : null // cb
                },
                PortalSettings.ActiveTab.FullUrl, // syst - ok
                PortalRoot = "//" + PortalSettings.PortalAlias.HTTPAlias + "/", // syst ok
                DefaultLanguageID = _sxcInstance?.EavAppContext.Dimensions.GetLanguageId(PortalSettings.DefaultLanguage) // unused /  syst - ok
            };
            return globData;
        }

    }

    public class ClientInfosAll
    {
        public ClientInfosEnvironment Environment;
        public ClientInfosUser User;
        public ClientInfosLanguages Language;
        public ClientInfoContentBlock ContentBlock; // todo: still not sure if these should be separate...
        public ClientInfoContentGroup ContentGroup;

        public ClientInfosAll(string systemRootUrl, PortalSettings ps, ModuleInstanceContext mic, SxcInstance sxc, UserInfo uinfo, int zoneId, bool isCreated)
        {
            Environment = new ClientInfosEnvironment(systemRootUrl, ps, mic);
            Language = new ClientInfosLanguages(ps, zoneId);
            User = new ClientInfosUser(uinfo);

            var tempVisibleStatus = DnnStuffToRefactor.TryToGetReliableSetting(sxc.ModuleInfo,
                Settings.SettingsShowTemplateChooser);
            var templateChooserVisible = bool.Parse(tempVisibleStatus ?? "true");
            ContentBlock = new ClientInfoContentBlock(templateChooserVisible, mic.ModuleId, null, 0);
            ContentGroup = new ClientInfoContentGroup(sxc, isCreated);
        }
    }

    public class ClientInfosEnvironment
    {
        public int WebsiteId;       // aka PortalId
        public string WebsiteUrl;
        // public string WebsiteVersion;

        public int PageId;          // aka TabId
        public string PageUrl;

        public int InstanceId;      // aka ModuleId

        public string SxcVersion;
        // public string SxcDialogUrl;
        public string SxcRootUrl;

        public ClientInfosEnvironment(string systemRootUrl, PortalSettings ps, ModuleInstanceContext mic)
        {
            WebsiteId = ps.PortalId;
            WebsiteUrl = "//" + ps.PortalAlias.HTTPAlias + "/";

            PageId = mic.TabId;
            PageUrl = ps.ActiveTab.FullUrl;

            InstanceId = mic.ModuleId;

            SxcVersion = Settings.Version.ToString();
            // SxcDialogUrl = Globals.NavigateURL(PageId);
            SxcRootUrl = systemRootUrl;
        }
    }

    public class ClientInfosUser
    {
        public bool CanDesign;
        public bool CanDevelop;

        public ClientInfosUser(UserInfo uinfo)
        {
            CanDesign = SecurityHelpers.IsInSexyContentDesignersGroup(uinfo);
            CanDevelop = uinfo.IsSuperUser;
        }
    }

    public class ClientInfosLanguages
    {
        public string Current;
        public string Primary;
        public IEnumerable<ClientInfoLanguage> All;

        public ClientInfosLanguages(PortalSettings ps, int zoneId)
        {
            Current = ps.CultureCode.ToLower();
            Primary = ps.DefaultLanguage.ToLower();
            All = ZoneHelpers.GetCulturesWithActiveState(ps.PortalId, zoneId)
                    .Where(c => c.Active)
                    .Select(c => new ClientInfoLanguage() { Key = c.Code.ToLower(), Name = c.Text });
        }
    }

    public class ClientInfoLanguage
    {
        public string Key;
        public string Name;
    }

    public class ClientInfoContentBlock //: ClientInfoEntity
    {
        public bool ShowTemplatePicker;
        // public bool GroupIsCreated;
        public bool ParentIsEntity;
        public int ParentId;
        public string ParentFieldName;
        public int ParentFieldSortOrder;

        public ClientInfoContentBlock(bool showSelector, /*bool groupIsCreated,*/ int parentId, string parentFieldName, int indexInField)
        {
            ShowTemplatePicker = showSelector;
            //GroupIsCreated = groupIsCreated;
            ParentIsEntity = false;
            ParentId = parentId;
            ParentFieldName = parentFieldName;
            ParentFieldSortOrder = indexInField;
        }
    };

    public class ClientInfoContentGroup: ClientInfoEntity
    {
        public bool IsCreated;
        public bool IsList;
        public int TemplateId;
        public string ContentTypeName;
        public string AppUrl;
        public bool AppIsContent;
        public bool HasContent;

        public ClientInfoContentGroup(SxcInstance sxc, bool isCreated)
        {
            IsCreated = isCreated;
            if (isCreated)
            {
                Id = sxc.ContentGroup.ContentGroupId;
                Guid = sxc.ContentGroup.ContentGroupGuid;
                AppId = sxc.AppId ?? 0;                     // todo: check if the 0 (previously null) causes problems
                AppUrl = sxc.App.Path + "/";
                HasContent = sxc.Template != null && sxc.ContentGroup.Exists;
            }
            ZoneId = sxc.ZoneId ?? 0;
            TemplateId = sxc.Template?.TemplateId ?? 0;     // todo: check if the 0 (previously null) causes problems
            ContentTypeName = sxc.Template?.ContentTypeStaticName ?? "";
            IsList = isCreated && sxc.ContentGroup.Content.Count > 1;
        }
    }

    public abstract class ClientInfoEntity
    {
        public int ZoneId;  // the zone of the content-block
        public int AppId;   // the zone of the content-block
        public Guid Guid;   // the entity-guid of the content-block
        public int Id;      // the entity-id of the content-block
    }


}