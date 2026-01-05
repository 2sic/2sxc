using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Sys;
using ToSic.Sys.Capabilities.Fingerprints;
using ToSic.Sys.Capabilities.Platform;

namespace ToSic.Sxc.WebApi.Sys.ExternalLinks;

/// <summary>
/// Service to generate links to getting started, app-details and more.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExternalLinksService(SystemFingerprint fingerprint, IPlatformInfo platformInfo)
{
    /// <summary>
    /// Link to a page in the 2sxc.org destination such as getting started, app-home, etc.
    /// </summary>
    /// <returns>a link</returns>
    public string LinkToDestination(ExternalSxcDestinations destination, ISite site, int moduleId, IAppSpecs? appSpecsOrNull, bool isContentApp)
    {
        var destinationPart = "";
        if (destination == ExternalSxcDestinations.AutoConfigure)
            destinationPart =
                $"&destination=autoconfigure{(isContentApp ? KnownAppsConstants.ContentAppAutoConfigureId : KnownAppsConstants.AppAutoConfigureId)}";
        else if (destination == ExternalSxcDestinations.Features) 
            destinationPart = "&destination=features";

        // ReSharper disable once StringLiteralTypo
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