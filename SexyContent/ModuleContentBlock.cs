using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public class ModuleContentBlock: IContentBlock
    {
        public int ZoneId { get;  }
        public int AppId { get; }

        public App App { get;  }

        public ModuleInfo ModuleInfo;
        //public NameValueCollection UrlParams;

        public bool UnreliableInfoThatSettingsAreStored; // todo
        public bool ContentGroupIsCreated;

        public bool ShowTemplatePicker { get; set; }
        public bool ParentIsEntity => false;
        public int ParentId { get; }
        public string ParentFieldName => null;
        public int ParentFieldSortOrder => 0;

        #region Template and extensive template-choice initialization
        private Template _template;
        private string TemplateDefinitionSource;

        // ensure the data is also set correctly...
        // Sequence of determining template
        // 3. If content-group exists, use template definition there
        // 4. If module-settings exists, use that
        // 5. If nothing exists, ensure system knows nothing applied 
        // #. possible override: If specifically defined in some object calls (like web-api), use that (set when opening this object?)
        // #. possible override in url - and allowed by permissions (admin/host), use that
        public Template Template
        {
            get 
            {
                return _template; 
            }
            set
            {
                _template = value;
                _dataSource = null; // reset this
            }
        }

        #endregion

        public PortalSettings AppOwnerPortalSettings ;

        private ViewDataSource _dataSource;
        public ViewDataSource Data
        {
            get
            {
                if (ModuleInfo == null)
                    throw new Exception("Can't get data source, module context unknown. Please only use this on a 2sxc-object which was initialized in a dnn-module context");

                return _dataSource ??
                       (_dataSource =
                           ViewDataSource.ForModule(ModuleInfo.ModuleID, SecurityHelpers.HasEditPermission(ModuleInfo), Template, SxcInstance));
            }
        }

        public ContentGroup ContentGroup { get; }

        public ModuleContentBlock(ModuleInfo moduleInfo, UserInfo userInfo, NameValueCollection queryString)
        {
            if(moduleInfo == null)
                throw new Exception("Need valid ModuleInfo / ModuleConfiguration of runtime");

            //UrlParams = queryString;
            ModuleInfo = moduleInfo;
            ParentId = moduleInfo.ModuleID;

            // Ensure we know what portal the stuff is coming from
            AppOwnerPortalSettings = moduleInfo.OwnerPortalID != moduleInfo.PortalID
                ? new PortalSettings(moduleInfo.OwnerPortalID)
                : PortalSettings.Current;

            // todo: #482 this override is an old mechanism which isn't needed any more when we use the web-api!, remove when we get rid of all those web controls
            // it's also not really part of the content-block, because it will help access OTHER infos of the app...
            // Set ZoneId based on the a) maybe URL (if super-user) or default-of-portal
            //var zoneId = ((userInfo?.IsSuperUser ?? false) && !string.IsNullOrEmpty(queryString["ZoneId"])
            //    ? int.Parse(queryString["ZoneId"])
            //    : ZoneHelpers.GetZoneID(moduleInfo.OwnerPortalID));
            //ZoneId = zoneId ?? 0;   // fallback/undefined
            ZoneId = ZoneHelpers.GetZoneID(moduleInfo.OwnerPortalID) ?? 0; // new
            

            // todo: #482 this still allows url-app-change, NOT needed any more when we use the web-api!, remove when we get rid of all those web controls
            // Set AppId base on a) name=2sxc> "content"-ID or b) url or c) module-setting or d) null
            var appId = AppHelpers.GetAppIdFromModule(moduleInfo);
            if (appId != null)
                UnreliableInfoThatSettingsAreStored = true;
            AppId = appId ?? 0;     // fallback/undefined

            if (AppId != 0)
            {
                // try to load the app - if possible
                App = new App(AppOwnerPortalSettings, AppId, ZoneId);
                ContentGroup = App.ContentGroupManager.GetContentGroupForModule(moduleInfo.ModuleID);

                // use the content-group template, which already covers stored data + module-level stored settings
                Template = ContentGroup.Template;

                // - in sxcinstance... CheckTemplateOverrides(); // check url-params, etc.

                // ensure data is initialized
                // nothing necessary, happens on the property

                // maybe ensure that App.Data is ready?
                // App.InitData(...)?
            }
        }

        private SxcInstance _sxcInstance;
        public SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this));

        public bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";

    }
}