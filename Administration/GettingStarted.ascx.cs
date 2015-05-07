using System;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.Administration
{
    public partial class GettingStarted : SexyControlAdminBase
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            var gettingStartedSrc = "http://gettingstarted.2sexycontent.org/router.aspx?";
            
            // Add DNN Version
            gettingStartedSrc += "DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4);
            // Add 2sxc Version
            gettingStartedSrc += "&2SexyContentVersion=" + SexyContent.ModuleVersion;
            // Add module type
            gettingStartedSrc += "&ModuleName=" + ModuleConfiguration.DesktopModule.ModuleName;
            // Add module id
            gettingStartedSrc += "&ModuleId=" + ModuleId;
            // Add Portal ID
            gettingStartedSrc += "&PortalID=" + PortalId;
            // Add VDB / Zone ID (if set)
            var ZoneID = SexyContent.GetZoneID(PortalId);
            gettingStartedSrc += ZoneID.HasValue ? "&ZoneID=" + ZoneID.Value : "";
            // Add AppStaticName and Version
            if (AppId.HasValue && !IsContentApp)
            {
                var app = SexyContent.GetApp(ZoneId.Value, AppId.Value, Sexy.OwnerPS);

                gettingStartedSrc += "&AppGuid=" + app.AppGuid;
                if (app.Configuration != null)
                {
                    gettingStartedSrc += "&AppVersion=" + app.Configuration.Version;
                    gettingStartedSrc += "&AppOriginalId=" + app.Configuration.OriginalId;
                }
            }
            // Add DNN Guid
            var HostSettings = HostController.Instance.GetSettingsDictionary();
            gettingStartedSrc += HostSettings.ContainsKey("GUID") ? "&DnnGUID=" + HostSettings["GUID"] : "";
            // Add Portal Default Language
            gettingStartedSrc += "&DefaultLanguage=" + PortalSettings.DefaultLanguage;
            // Add current language
            gettingStartedSrc += "&CurrentLanguage=" + PortalSettings.CultureCode;

            // Set src to iframe
            ifrGettingStarted.Attributes["src"] = gettingStartedSrc;
        }
    }
}