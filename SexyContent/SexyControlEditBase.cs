using System;
using System.Collections.Specialized;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;
using ToSic.SexyContent.Administration;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Contains properties that all controls use that edit the current module's data (not global data like admin controls)
    /// It delivers a context that uses the current modules App and the current portal's Zone.
    /// </summary>
    public abstract class SexyControlEditBase : PortalModuleBase
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // Set ZoneId based on the context
            var zoneId = (!UserInfo.IsSuperUser
                           ? ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID)
                           : !string.IsNullOrEmpty(Request.QueryString["ZoneId"])
                               ? int.Parse(Request.QueryString["ZoneId"]) : ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID));

            // Set AppId based on the context
            var appId = AppHelpers.GetAppIdFromModule(ModuleConfiguration);
            if (appId != null)
                SettingsAreStored = true;

            // possibly get app-id from url, but only if on an admin-control
            // the only reason the code is currently here, is because the admin controls will be removed soon (replaced by JS-UIs)
            if (this is SexyControlAdminBaseWillSoonBeRemoved)
            {
                var appIdString = Request.QueryString[SexyContent.Settings.AppIDString];
                int appId2;
                if (appIdString != null && int.TryParse(appIdString, out appId2))
                    appId = appId2;
            }

            // Init SxcContext based on zone/app
            //if (zoneId.HasValue && appId.HasValue)
                SxcContext = new InstanceContext(zoneId ?? 0, appId ?? 0, true, ModuleConfiguration.OwnerPortalID,
                    ModuleContext.Configuration);
        }


        #region basic properties like Sexy, App, Zone, etc.
        protected InstanceContext SxcContext { get; set; }

        protected int? ZoneId => SxcContext?.ZoneId ?? 0;

        protected int? AppId => SettingsAreStored ? SxcContext?.AppId : null; // some systems rely on appid being blank to adapt behaviour if nothing is saved yet

        protected bool SettingsAreStored;
        #endregion

        public bool IsContentApp => SxcContext.IsContentApp;// ModuleConfiguration.DesktopModule.ModuleName == "2sxc";

        // private ContentGroup _contentGroup;
        protected ContentGroup ContentGroup => SxcContext.ContentGroup; //  _contentGroup ?? (_contentGroup = SxcContext.ContentGroupManager.GetContentGroupForModule(ModuleConfiguration.ModuleID));

        #region template loading and stuff...

        protected Template Template => SxcContext.Template;

        #endregion




    }
}