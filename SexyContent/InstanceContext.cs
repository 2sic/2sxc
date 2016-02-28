using System;
using System.Configuration;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Interfaces;
using ToSic.SexyContent.Statics;

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
        /// The Content Data Context pointing to a full EAV
        /// </summary>
        public EavDataController ContentContext;

        internal int? ZoneId { get; set; }

        public int? AppId { get; private set; }

        public Templates Templates { get; internal set; }

		public ContentGroups ContentGroups { get; internal set; }

        private App _app;
        public App App {
            get { return _app ?? (_app = AppHelpers.GetApp(ZoneId.Value, AppId.Value, PortalSettingsOfOriginalModule)); }
        }

        /// <summary>
        /// Environment - should be the place to refactor everything into, which is the host around 2sxc
        /// </summary>
        public Environment.Environment Environment = new Environment.Environment();

        internal ModuleInfo ModuleInfo { get; private set; }

        /// <summary>
        /// This returns the PS of the original module. When a module is mirrored across portals,
        /// then this will be different from the PortalSettingsOfVisitedPage, otherwise they are the same
        /// </summary>
        internal PortalSettings PortalSettingsOfOriginalModule { get; set; }

        public ViewDataSource DataSource
        {
            get
            {
                if(ModuleInfo == null)
                    throw new Exception("Can't get data source, module context unknown. Please only use this on a 2sxc-object which was initialized in a dnn-module context");
                return ViewDataSource.ForModule(ModuleInfo.ModuleID, SecurityHelpers.HasEditPermission(ModuleInfo), ContentGroups.GetContentGroupForModule(ModuleInfo.ModuleID).Template, this);
            }
        }

        #endregion


        #region SexyContent Constructor


        /// <summary>
        /// Instanciates Content and Template-Contexts
        /// </summary>
        public InstanceContext(int zoneId, int appId, bool enableCaching = true, int? ownerPortalId = null, ModuleInfo moduleInfo = null)
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

            // Only disable caching of templates and contentgroupitems
            // if AppSetting "ToSIC_SexyContent_EnableCaching" is disabled
            if(enableCaching)
            {
                var cachingSetting = ConfigurationManager.AppSettings["ToSIC_SexyContent_EnableCaching"];
                if (!String.IsNullOrEmpty(cachingSetting) && cachingSetting.ToLower() == "false")
                    enableCaching = false;
            }

            Templates = new Templates(zoneId, appId);
			ContentGroups = new ContentGroups(zoneId, appId);

            // Set Properties on ContentContext
            ContentContext = EavDataController.Instance(zoneId, appId); // EavContext.Instance(zoneId, appId);
            ContentContext.UserName = (HttpContext.Current == null || HttpContext.Current.User == null) ? Settings.InternalUserName : HttpContext.Current.User.Identity.Name;

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
    }
}