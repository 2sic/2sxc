using ToSic.Eav;
using ToSic.Eav.Apps.Internal.Specs;
using ToSic.Eav.Context;
using ToSic.Eav.Security.Fingerprint;

namespace ToSic.Sxc.Integration.Installation;

/// <summary>
/// WIP - single location for building router links for installer and app/content infos
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RemoteRouterLink(SystemFingerprint fingerprint, IPlatformInfo platformInfo)
{
    public string LinkToRemoteRouter(RemoteDestinations destination, ISite site, int moduleId, IAppSpecs appSpecsOrNull, bool isContentApp)
    {
        var destinationPart = "";
        if (destination == RemoteDestinations.AutoConfigure)
            destinationPart =
                $"&destination=autoconfigure{(isContentApp ? Eav.Constants.ContentAppAutoConfigureId : Eav.Constants.AppAutoConfigureId)}";
        else if (destination == RemoteDestinations.Features) 
            destinationPart = "&destination=features";

        var link = "//gettingstarted.2sxc.org/router.aspx?"
                   + $"Platform={platformInfo.Name}"
                   // note: Version ToString max 3, as Oqtane only has 3 version numbers, otherwise error
                   + $"&SysVersion={platformInfo.Version.ToString(3)}"
                   + $"&SxcVersion={EavSystemInfo.VersionString}"
                   + destinationPart
                   + "&ModuleId=" + moduleId
                   + "&SiteId=" + site?.Id
                   + "&ZoneID=" + site?.ZoneId
                   + "&DefaultLanguage=" + site?.DefaultCultureCode
                   + "&CurrentLanguage=" + site?.SafeCurrentCultureCode()
                   + "&SysGuid=" + platformInfo.Identity
            ;

        link += "&AppId=" + (isContentApp ? "Default" : appSpecsOrNull?.NameId ?? "");
            
        // Add AppStaticName and Version if _not_ the primary content-app
        if (appSpecsOrNull?.Configuration != null)
            link += $"&AppVersion={appSpecsOrNull.Configuration.Version}"
                    + $"&AppOriginalId={appSpecsOrNull.Configuration.OriginalId}";

        link += "&fp=" + System.Net.WebUtility.UrlEncode(fingerprint.GetFingerprint())?.ToLowerInvariant();
        return link;
    }

}

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public enum RemoteDestinations
{
    AutoConfigure,
    GettingStarted,
    Features
}