using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public class ModuleContentBlock
    {
        public int ZoneId;
        public int AppId;

        public App App;

        public ModuleInfo ModuleInfo;
        public NameValueCollection UrlParams;

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
        public Template Template
        {
            get 
            {
                return _template; 
            }
            internal set
            {
                _template = value;
                _dataSource = null; // reset this
            }
        }

        // Sequence of determining template
        // 1. If specifically defined in some object calls (like web-api), use that (set when opening this object?)
        // 2. If overriden in url - and allowed by permissions (admin/host), use that
        // 3. If content-group exists, use template definition there
        // 4. If module-settings exists, use that
        // 5. If nothing exists, ensure system knows nothing applied 

        //private void InitializeTemplateAndData()
        //{
        //    // check if we already know the app, because of not, no template selection is possible
        //    if (AppId == 0)
        //        return ;

        //    // var template = CheckTemplateOverrides();

        //    // now initialized data
        //    // 1. if already set, ok
        //    // 2. if it has a attached content-group, use that
        //    var maybeGuid = ModuleInfo.ModuleSettings[Settings.ContentGroupGuidString];
        //    Guid groupGuid;
        //    Guid.TryParse(maybeGuid?.ToString(), out groupGuid);

        //    // 3. if the module-settings knows what kind of content-group to simulate, use that
        //    // 4. otherwise, use nothing?

        //    //return template;
        //}

        private void CheckTemplateOverrides()
        {
            // _templateChecked = true; // assume this will be the case

            // #2 Change Template if URL contains the part in the metadata "ViewNameInUrl"
            if (AllowAutomaticTemplateChangeBasedOnUrlParams)
            {
                var urlParams = HttpContext.Current.Request.QueryString; // todo: reduce dependency on context-current...
                var templateFromUrl = TryToGetTemplateBasedOnUrlParams(urlParams);
                if (templateFromUrl != null)
                    _template = templateFromUrl;
            }

            // #3 check content-group 
            // already handled in default
            //if (ContentGroup != null)
            //    return _template = ContentGroup.Template;

            // #4 check stored module settings
            // currently handled in default
            //var previewTemplateString = ModuleInfo.ModuleSettings[Settings.PreviewTemplateIdString]?.ToString();
            //if (previewTemplateString != null)
            //{
            //    var tGuid = Guid.Parse(previewTemplateString);
            //    return App.TemplateManager.GetTemplate(tGuid);
            //}
            //// else: nothing found
            //_templateChecked = true;
            //return null;
        }

        /// <summary>
        /// combine all QueryString Params to a list of key/value lowercase and search for a template having this ViewNameInUrl
        /// QueryString is never blank in DNN so no there's no test for it
        /// </summary>
        private Template TryToGetTemplateBasedOnUrlParams(NameValueCollection urlParams) 
        {
            var urlParameterDict = urlParams.AllKeys.ToDictionary(key => key?.ToLower() ?? "", key => string.Format("{0}/{1}", key, urlParams[key]).ToLower());

            foreach (var template in App.TemplateManager.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
            {
                var desiredFullViewName = template.ViewNameInUrl.ToLower();
                if (desiredFullViewName.EndsWith("/.*"))   // match details/.* --> e.g. details/12
                {
                    var keyName = desiredFullViewName.Substring(0, desiredFullViewName.Length - 3);
                    if (urlParameterDict.ContainsKey(keyName))
                        return template;
                }
                else if (urlParameterDict.ContainsValue(desiredFullViewName)) // match view/details
                    return template;
            }

            return null;
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

        public ModuleContentBlock(ModuleInfo moduleInfo, UserInfo userInfo, NameValueCollection queryString, bool allowUrlOverrides)
        {
            if(moduleInfo == null)
                throw new Exception("Need valid ModuleInfo / ModuleConfiguration of runtime");

            UrlParams = queryString;
            ModuleInfo = moduleInfo;
            ParentId = moduleInfo.ModuleID;

            // Ensure we know what portal the stuff is coming from
            AppOwnerPortalSettings = moduleInfo.OwnerPortalID != moduleInfo.PortalID
                ? new PortalSettings(moduleInfo.OwnerPortalID)
                : PortalSettings.Current;

            // Set ZoneId based on the a) maybe URL (if super-user) or default-of-portal
            var zoneId = (allowUrlOverrides &&  (userInfo?.IsSuperUser ?? false) && !string.IsNullOrEmpty(UrlParams["ZoneId"])
                ? int.Parse(UrlParams["ZoneId"])
                : ZoneHelpers.GetZoneID(moduleInfo.OwnerPortalID));
            ZoneId = zoneId ?? 0;   // fallback/undefined

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

                CheckTemplateOverrides(); // check url-params, etc.

                // ensure data is initialized
                // nothing necessary, happens on the property

                // maybe ensure that App.Data is ready?
                // App.InitData(...)?
            }
        }

        private SxcInstance _sxcInstance;

        public SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(ZoneId, AppId, ModuleInfo));

        public bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";
        private bool AllowAutomaticTemplateChangeBasedOnUrlParams => !IsContentApp;  

    }
}