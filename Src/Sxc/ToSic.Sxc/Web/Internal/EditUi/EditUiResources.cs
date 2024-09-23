using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Services;
using ToSic.Eav.Integration;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Sxc.Services;
using static ToSic.Eav.Apps.AppStackConstants;
using static ToSic.Sxc.Web.WebResources.WebResourceConstants;
using static ToSic.Sxc.Web.WebResources.WebResourceProcessor;

namespace ToSic.Sxc.Web.Internal.EditUi;

/// <summary>
/// Provide all resources (fonts, icons, etc.) needed for the edit-ui
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EditUiResources(AppDataStackService stackServiceHelper, IZoneMapper zoneMapper, IFeaturesService features)
    : ServiceBase("Sxc.EUiRes", connect: [stackServiceHelper, zoneMapper, features])
{

    #region Resources / Constants

    public const string LinkTagTemplate = "<link rel=\"stylesheet\" href=\"{0}\">";
    public const string RobotoFromGoogle = "https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap";
    public const string RobotoFromAltCdn = "/google-fonts/roboto/fonts.css";

    // 2024-08-20 2dm: switching to Material Symbols
    // not quite done, must finish by ensuring the CDN solution works too...
    //<link rel = "stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200" />
    public const string
        MaterialIconsGoogle = // "https://fonts.googleapis.com/icon?family=Material+Icons|Material+Icons+Outlined";
            "https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200";
    public const string MaterialIconsAltCdn = "/google-fonts/material-icons/fonts.css";

    #endregion

    public EditUiResourcesSpecs GetResources(bool enabled, int? siteId, EditUiResourceSettings settings)
    {
        if (!enabled) return new();
        var cdnRoot = "";
        var useAltCdn = false;
        var html = "";

        if (features.IsEnabled(SxcFeatures.CdnSourceEdit.NameId) && siteId.HasValue)
        {
            var zoneId = zoneMapper.GetZoneId(siteId.Value);
            var stack = stackServiceHelper.InitForPrimaryAppOfZone(zoneId).GetStack(RootNameSettings);
            var getResult = stack.InternalGetPath($"{WebResourcesNode}.{CdnSourceEditField}");
            cdnRoot = getResult.Result as string;
            useAltCdn = cdnRoot.HasValue() && cdnRoot != CdnDefault;
            cdnRoot += VersionSuffix;
            html += $"<!-- CDN settings {getResult.IsFinal}, '{getResult.Result}', '{getResult.Result?.GetType()}' '{cdnRoot}', {cdnRoot?.Length} -->";
        }
        else
            html += "<!-- CDN Settings: Default (feature not enabled) -->";

        if (settings.IconsMaterial)
        {
            var url = useAltCdn ? cdnRoot + MaterialIconsAltCdn : MaterialIconsGoogle;
            html += "\n" + string.Format(LinkTagTemplate, url);
        }

        if (settings.FontRoboto)
        {
            var url = useAltCdn ? cdnRoot + RobotoFromAltCdn : RobotoFromGoogle;
            html += "\n" + string.Format(LinkTagTemplate, url);
        }
        html += "\n";
        return new() { HtmlHead = html };
    }

    public class EditUiResourcesSpecs
    {
        public string HtmlHead { get; set; } = "";

        // later we'll also add CSP specs here
    }

}