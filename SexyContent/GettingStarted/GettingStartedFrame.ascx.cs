using System;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Framework;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.GettingStarted
{
    public partial class GettingStartedFrame : SexyControlEditBase
    {
        public int ModuleID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            GettingStartedUrl();
        }

        public string GettingStartedUrl()
        {
            var gettingStartedSrc = "http://gettingstarted.2sexycontent.org/router.aspx?";

            // Add desired destination
            gettingStartedSrc += "destination=" +
                                 (ModuleConfiguration.DesktopModule.ModuleName == "2sxc"
                                     ? "autoconfigurecontent"
                                     : "autoconfigureapp");

            // Add DNN Version
            gettingStartedSrc += "&DnnVersion=" + Assembly.GetAssembly(typeof (Globals)).GetName().Version.ToString(4);
            // Add 2SexyContent Version
            gettingStartedSrc += "&2SexyContentVersion=" + SexyContent.ModuleVersion;
            // Add module type
            gettingStartedSrc += "&ModuleName=" + ModuleConfiguration.DesktopModule.ModuleName;
            // Add module id
            gettingStartedSrc += "&ModuleId=" + ModuleID;
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
            return gettingStartedSrc;
        }
    }
}