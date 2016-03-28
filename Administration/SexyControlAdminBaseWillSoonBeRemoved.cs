using System;
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
    public abstract class SexyControlAdminBaseWillSoonBeRemoved : PortalModuleBase //SexyControlEditBase
    {
        internal int? ZoneId;
        internal int? AppId;

        internal App App;

        internal bool IsContentApp;

        protected void Page_Init(object sender, EventArgs e)
        {
            ZoneId = ((UserInfo?.IsSuperUser ?? false) && !string.IsNullOrEmpty(Request.QueryString["ZoneId"])
                ? int.Parse(Request.QueryString["ZoneId"])
                : ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID));

            AppId = AppHelpers.GetAppIdFromModule(ModuleConfiguration); // todo: this calso contains the url-lookup

            App = new App(PortalSettings, AppId.Value, ZoneId.Value);

            IsContentApp = ModuleConfiguration.DesktopModule.ModuleName == "2sxc";
        }
    }
}