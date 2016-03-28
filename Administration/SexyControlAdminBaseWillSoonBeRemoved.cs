using System;
using System.Web;
using DotNetNuke.Entities.Modules;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.Administration
{
    /// <summary>
    /// Contains properties that all controls use that edit the current module's data (not global data like admin controls)
    /// It delivers a context that uses the current modules App and the current portal's Zone.
    /// 
    /// Important: this class is just a layer between the main control and the admin-UIs
    /// It's needed, because there is a type-detection which allows a few more things - in the other class.
    /// This not-ideal solution is temporaryr, because soon there will be no more webforms-UIs
    /// </summary>
    public abstract class SexyControlAdminBaseWillSoonBeRemoved : PortalModuleBase 
    {
        public int? ZoneId;
        public int? AppId;

        public App App;

        public bool IsContentApp;

        protected void Page_Init(object sender, EventArgs e)
        {
            ZoneId = ((UserInfo?.IsSuperUser ?? false) && !string.IsNullOrEmpty(Request.QueryString["ZoneId"])
                ? int.Parse(Request.QueryString["ZoneId"])
                : ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID));

            // try to get the app-id from the url - else get from current module-config
            string appIdString = null;
            int appId;
            if (!string.IsNullOrEmpty(Request.QueryString["AppId"]))
                appIdString = Request.QueryString["AppId"];
            if (appIdString != null && int.TryParse(appIdString.Split(',')[0], out appId))
                AppId = appId;
            // NO else - it only works with the app-id from the url
            //else
            //    AppId = AppHelpers.GetAppIdFromModule(ModuleConfiguration); 

            App = new App(PortalSettings, AppId.Value, ZoneId.Value);

            IsContentApp = ModuleConfiguration.DesktopModule.ModuleName == "2sxc";
        }
    }
}