using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent.Administration
{
    public partial class GettingStarted : SexyControlAdminBase
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            var GettingStartedSrc = "http://gettingstarted.2sexycontent.org/router.aspx?";
            
            // Add DNN Version
            GettingStartedSrc += "DnnVersion=" + Assembly.GetAssembly(typeof(DotNetNuke.Common.Globals)).GetName().Version.ToString(4);
            // Add 2SexyContent Version
            GettingStartedSrc += "&2SexyContentVersion=" + SexyContent.ModuleVersion;
            // Add Portal ID
            GettingStartedSrc += "&PortalID=" + PortalId.ToString();
            // Add VDB / Zone ID (if set)
            var ZoneID = SexyContent.GetZoneID(PortalId);
            GettingStartedSrc += ZoneID.HasValue ? "&ZoneID=" + ZoneID.Value.ToString() : "";
            // Add AppStaticName and Version
            if (AppId.HasValue)
            {
                var app = SexyContent.GetApp(ZoneId.Value, AppId.Value);
                GettingStartedSrc += "&AppGuid=" + app.AppGuid;
                if (app.Configuration != null)
                {
                    GettingStartedSrc += "&AppVersion=" + app.Configuration.Version;
                    GettingStartedSrc += "&AppOriginalId=" + app.Configuration.OriginalId;
                }
            }
            // Add DNN Guid
            var HostSettings = HostController.Instance.GetSettingsDictionary();
            GettingStartedSrc += HostSettings.ContainsKey("GUID") ? "&DnnGUID=" + HostSettings["GUID"].ToString() : "";
            // Add Portal Default Language
            GettingStartedSrc += "&DefaultLanguage=" + PortalSettings.DefaultLanguage;

            // Set src to iframe
            ifrGettingStarted.Attributes["src"] = GettingStartedSrc;
        }
    }
}