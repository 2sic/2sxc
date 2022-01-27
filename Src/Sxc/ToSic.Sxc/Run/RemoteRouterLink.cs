using ToSic.Eav;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Fingerprint;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Run
{
    /// <summary>
    /// WIP - single location for building router links for installer and app/content infos
    /// </summary>
    [PrivateApi]
    public class RemoteRouterLink
    {
        public RemoteRouterLink(SystemFingerprint fingerprint, IPlatformInfo platformInfo)
        {
            _fingerprint = fingerprint;
            _platformInfo = platformInfo;
        }
        private readonly SystemFingerprint _fingerprint;
        private readonly IPlatformInfo _platformInfo;

        public string LinkToRemoteRouter(RemoteDestinations destination, ISite site, int moduleId, IApp app, bool isContentApp)
        {
            var destinationPart = "";
            if (destination == RemoteDestinations.AutoConfigure)
                destinationPart =
                    $"&destination=autoconfigure{(isContentApp ? Eav.Constants.ContentAppAutoConfigureId : Eav.Constants.AppAutoConfigureId)}";
            else if (destination == RemoteDestinations.Features) 
                destinationPart = "&destination=features";

            var link = "//gettingstarted.2sxc.org/router.aspx?"
                        + $"Platform={_platformInfo.Name}"
                        // note: Version ToString max 3, as Oqtane only has 3 version numbers, otherwise error
                        + $"&SysVersion={_platformInfo.Version.ToString(3)}"
                        + $"&SxcVersion={EavSystemInfo.VersionString}"
                        + destinationPart
                        + "&ModuleId=" + moduleId
                        + "&SiteId=" + site?.Id
                        + "&ZoneID=" + site?.ZoneId
                        + "&DefaultLanguage=" + site?.DefaultCultureCode
                        + "&CurrentLanguage=" + site?.SafeCurrentCultureCode()
                        + "&SysGuid=" + _platformInfo.Identity
                ;

            link += "&AppId=" + (isContentApp ? "Default" : app?.NameId ?? "");
            
            // Add AppStaticName and Version if _not_ the primary content-app
            if (app?.Configuration != null)
                link += $"&AppVersion={app.Configuration.Version}"
                        + $"&AppOriginalId={app.Configuration.OriginalId}";

            link += "&fp=" + System.Net.WebUtility.UrlEncode(_fingerprint.GetFingerprint())?.ToLowerInvariant();
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
