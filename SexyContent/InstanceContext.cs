using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    /// <summary>
    /// This is an instance-context of a Content-Module. It basically encapsulates the instance-state, incl.
    /// IDs of Zone & App, the App, EAV-Context, Template, Content-Groups (if available), Environment and OriginalModule (in case it's from another portal)
    /// It is needed for just about anything, because without this set of information
    /// it would be hard to get anything done .
    /// Note that it also adds the current-user to the state, so that the system can log data-changes to this user
    /// </summary>
    public class InstanceContext 
    {
        #region Properties
        /// <summary>
        /// The Content Data Context pointing to a full EAV, pre-configured for this specific App
        /// </summary>
        public EavDataController EavAppContext;

        internal int? ZoneId { get; set; }

        public int? AppId { get; }

        public TemplateManager AppTemplates { get; set; }

		public ContentGroupManager AppContentGroups { get; set; }

        private ContentGroup _contentGroup;
        internal ContentGroup ContentGroup => _contentGroup ??
                                               (_contentGroup = AppContentGroups.GetContentGroupForModule(ModuleInfo.ModuleID));

        private App _app;
        public App App => _app ?? (_app = AppHelpers.GetApp(ZoneId.Value, AppId.Value, PortalSettingsOfOriginalModule));

        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public Environment.Environment Environment = new Environment.Environment();

        internal ModuleInfo ModuleInfo { get; }

        public bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";


        /// <summary>
        /// This returns the PS of the original module. When a module is mirrored across portals,
        /// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        /// </summary>
        internal PortalSettings PortalSettingsOfOriginalModule { get; set; }

        private ViewDataSource _dataSource;
        public ViewDataSource DataSource
        {
            get
            {
                if(ModuleInfo == null)
                    throw new Exception("Can't get data source, module context unknown. Please only use this on a 2sxc-object which was initialized in a dnn-module context");

                return _dataSource ??
                       (_dataSource =
                           ViewDataSource.ForModule(ModuleInfo.ModuleID, SecurityHelpers.HasEditPermission(ModuleInfo), Template, this));
            }
        }


        #endregion

        #region current template
        // todo: try to refactor most of the template-stuff out of this class again

        private bool AllowAutomaticTemplateChangeBasedOnUrlParams => !IsContentApp; 
        private Template _template;
        private bool _templateLoaded;
        internal Template Template
        {
            get
            {
                if (_template != null || _templateLoaded) return _template;

                if (!AppId.HasValue)
                    return null;

                // Change Template if URL contains "ViewNameInUrl"
                if (AllowAutomaticTemplateChangeBasedOnUrlParams)
                {
                    var urlParams =  HttpContext.Current.Request.QueryString;
                    var templateFromUrl = TryToGetTemplateBasedOnUrlParams(urlParams);
                    if (templateFromUrl != null)
                        _template = templateFromUrl;
                }
                if (_template == null)
                    _template = ContentGroup.Template;
                _templateLoaded = true;
                return _template;
            }
            set
            {
                _template = value;
                _templateLoaded = true;
                _dataSource = null; // reset this
            }
        }

        /// <summary>
        /// combine all QueryString Params to a list of key/value lowercase and search for a template having this ViewNameInUrl
        /// QueryString is never blank in DNN so no there's no test for it
        /// </summary>
        private Template TryToGetTemplateBasedOnUrlParams(NameValueCollection urlParams)
        {
            var urlParameterDict = urlParams.AllKeys.ToDictionary(key => key?.ToLower() ?? "", key => string.Format("{0}/{1}", key, urlParams[key]).ToLower());

            foreach (var template in AppTemplates.GetAllTemplates().Where(t => !string.IsNullOrEmpty(t.ViewNameInUrl)))
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

        #region Constructor


        /// <summary>
        /// Instanciates Content and Template-Contexts
        /// </summary>
        internal InstanceContext(int zoneId, int appId, bool enableCaching = true, int? ownerPortalId = null, ModuleInfo moduleInfo = null)
        {
            ModuleInfo = moduleInfo;
            PortalSettingsOfOriginalModule = ownerPortalId.HasValue ? new PortalSettings(ownerPortalId.Value) : PortalSettings.Current;

            if (zoneId == 0)
                if (PortalSettingsOfOriginalModule == null || !ZoneHelpers.GetZoneID(PortalSettingsOfOriginalModule.PortalId).HasValue)
                    zoneId = Constants.DefaultZoneId;
                else
                    zoneId = ZoneHelpers.GetZoneID(PortalSettingsOfOriginalModule.PortalId).Value;

            if (appId == 0)
                appId = AppHelpers.GetDefaultAppId(zoneId);

            AppTemplates = new TemplateManager(zoneId, appId);
			AppContentGroups = new ContentGroupManager(zoneId, appId);

            // Set Properties on ContentContext
            EavAppContext = EavDataController.Instance(zoneId, appId); // EavContext.Instance(zoneId, appId);
            EavAppContext.UserName = (HttpContext.Current == null || HttpContext.Current.User == null) ? Settings.InternalUserName : HttpContext.Current.User.Identity.Name;

            ZoneId = zoneId;
            AppId = appId;


            #region Prepare Environment information 
            // 2016-01 2dm - this is new, the environment is where much code should go to later on

            // Build up the environment. If we know the module context, then use permissions from there
            Environment.Permissions = (moduleInfo != null)
                ? (IPermissions) new Environment.Dnn7.Permissions(moduleInfo)
                : new Environment.None.Permissions();
            #endregion
        }


        #endregion

        #region RenderEngine

        public IEngine RenderingEngine(InstancePurposes renderingPurpose)
        {
            var engine = EngineFactory.CreateEngine(Template);
            engine.Init(Template, App, ModuleInfo, DataSource, renderingPurpose, this);
            return engine;
        }

        #endregion
    }
}