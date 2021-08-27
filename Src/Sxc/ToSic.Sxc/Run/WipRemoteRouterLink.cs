using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Run
{
    /// <summary>
    /// WIP - single location for building router links for installer and app/content infos
    /// </summary>
    [PrivateApi]
    public class WipRemoteRouterLink
    {
        public IFingerprint Fingerprint { get; }

        public WipRemoteRouterLink(IFingerprint fingerprint)
        {
            Fingerprint = fingerprint;
        }
        
        public string LinkToRemoteRouter(RemoteDestinations destination, 
            string platform, string sysVersion, string sysGuid, 
            ISite site, int moduleId, IApp app, bool isContentApp)
        {
            var destinationPart = "";
            if (destination == RemoteDestinations.AutoConfigure)
                destinationPart =
                    $"&destination=autoconfigure{(isContentApp ? Eav.Constants.ContentAppName.ToLowerInvariant() : "app")}";
            else if (destination == RemoteDestinations.Features) 
                destinationPart = "&destination=features";

            var link = "//gettingstarted.2sxc.org/router.aspx?"
                        + $"Platform={platform}"
                        + $"&SysVersion={sysVersion}"
                        + $"&SxcVersion={Settings.ModuleVersion}"
                        + destinationPart
                        + "&ModuleId=" + moduleId
                        + "&SiteId=" + site?.Id
                        + "&ZoneID=" + site?.ZoneId
                        + "&DefaultLanguage=" + site?.DefaultCultureCode
                        + "&CurrentLanguage=" + site?.SafeCurrentCultureCode()
                        + "&SysGuid=" + sysGuid
                ;

            link += "&AppId=" + (isContentApp ? "Default" : app?.AppGuid ?? "");
            
            // Add AppStaticName and Version if _not_ the primary content-app
            if (app?.Configuration != null)
                link += $"&AppVersion={app.Configuration.Version}"
                        + $"&AppOriginalId={app.Configuration.OriginalId}";

            link += "&fp=" + System.Net.WebUtility.UrlEncode(Fingerprint.GetSystemFingerprint())?.ToLowerInvariant();
            return link;
        }

    }

    public enum RemoteDestinations
    {
        AutoConfigure,
        GettingStarted,
        Features
    }
}
