using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;
using static ToSic.Sxc.Web.Internal.ClientAssets.ClientAssetConstants;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.Web.Internal.PageService;

/// <summary>
/// This should bundle all the page changes once a module is done.
/// Usually used at the top-level of render-result, and in future also on page-level dynamic code
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageChangeSummary(
    LazySvc<IBlockResourceExtractor> resourceExtractor,
    LazySvc<RequirementsService> requirements)
    : ServiceBase(SxcLogName + "PgChSm", connect: [requirements, resourceExtractor])
{
    public IRenderResult FinalizeAndGetAllChanges(PageServiceShared pss, bool enableEdit)
    {
        var l = Log.Fn<IRenderResult>(timer: true);
        if (enableEdit)
        {
            pss.Activate(SxcPageFeatures.ToolbarsInternal.NameId);
            pss.Activate(SxcPageFeatures.ToolbarsAutoInternal.NameId);
        }

        var assets = pss.Assets;
        var (newAssets, rest) = ConvertSettingsAssetsIntoReal(pss.PageFeatures.FeaturesFromSettingsGetNew(Log));

        assets.AddRange(newAssets);
        assets = [.. assets.OrderBy(a => a.PosInPage)];

        // Collect Warnings of page features which may require other features enabled
        var features = pss.PageFeatures.GetFeaturesWithDependentsAndFlush(Log);

        var errors = requirements.Value
            .Check(features)
            .Select(f => f.Message)
            .ToList();

        var result = new RenderResult(null)
        {
            Assets = assets,
            FeaturesFromSettings = rest,
            Features = features,
            HeadChanges = pss.GetHeadChangesAndFlush(Log),
            PageChanges = pss.GetPropertyChangesAndFlush(Log),
            HttpStatusCode = pss.HttpStatusCode,
            HttpStatusMessage = pss.HttpStatusMessage,
            HttpHeaders = pss.HttpHeaders,

            // CSP settings
            CspEnabled = pss.Csp.IsEnabled,
            CspEnforced = pss.Csp.IsEnforced,
            CspParameters = pss.Csp.CspParameters(),
            Errors = errors,
        };

        // Whitelist any assets which were officially ok, or which were from the settings
        var additionalCsp = GetCspListFromAssets(assets);
        if (additionalCsp != null)
            result.CspParameters.Add(additionalCsp);

        return l.Return(result);
    }



    private (List<IClientAsset> newAssets, List<IPageFeature> rest) ConvertSettingsAssetsIntoReal(List<PageFeatureFromSettings> featuresFromSettings)
    {
        var l = Log.Fn<(List<IClientAsset> newAssets, List<IPageFeature> rest)>($"{featuresFromSettings.Count}");
        var newAssets = new List<IClientAsset>();
        foreach (var settingFeature in featuresFromSettings)
        {
            var autoOpt = settingFeature.AutoOptimize;
            var extracted = resourceExtractor.Value.Process(settingFeature.Html, new(
                css: new(autoOpt, AddToBottom, CssDefaultPriority, false, false),
                js: new(autoOpt, AddToBottom, JsDefaultPriority, autoOpt, autoOpt)));
            l.A($"Feature: {settingFeature.Name} - assets extracted: {extracted.Assets.Count}");
            if (!extracted.Assets.Any())
                continue;

            //// todo: If the original settings say we should auto-optimize, do this here
            //if (settingFeature.AutoOptimize)
            //    extracted.Assets.ForEach(a =>
            //    {
            //        if (!a.IsJs) return;
            //        a.HtmlAttributes["defer"] = "test-defer";
            //        a.HtmlAttributes["async"] = "test-async";
            //    });

            // All resources from the settings are seen as safe
            extracted.Assets.ForEach(a => a.WhitelistInCsp = true);

            newAssets.AddRange(extracted.Assets);

            // Reset the HTML to what's left after extracting the resources
            settingFeature.Html = extracted.Html;
        }

        var featsLeft = featuresFromSettings
            .Where(f => !string.IsNullOrWhiteSpace(f.Html))
            .Cast<IPageFeature>()
            .ToList();

        return l.Return((newAssets, featsLeft), $"New: {newAssets.Count}; Rest: {featsLeft.Count}");
    }

    private static CspParameters GetCspListFromAssets(IReadOnlyCollection<IClientAsset> assets)
    {
        if (assets == null || assets.Count == 0) return null;
        var toWhitelist = assets
            .Where(a => a.WhitelistInCsp)
            .Where(a => !a.Url.NeverNull().StartsWith("/")) // skip local files
            .ToList();
        if (!toWhitelist.Any()) return null;
        var whitelist = new CspParameters();
        foreach (var asset in toWhitelist)
            whitelist.Add((asset.IsJs ? "script" : "style") + "-src", asset.Url);

        return whitelist;
    }

}