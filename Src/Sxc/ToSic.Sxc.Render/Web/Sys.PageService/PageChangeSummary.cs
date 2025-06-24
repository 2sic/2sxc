
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.PageServiceShared.Internal;
using ToSic.Sys.Requirements;
using static ToSic.Sxc.Web.Internal.ClientAssets.ClientAssetConstants;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.Web.Internal.PageService;

/// <summary>
/// This should bundle all the page changes once a module is done.
/// Usually used at the top-level of render-result, and in future also on page-level dynamic code
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PageChangeSummary(
    LazySvc<IBlockResourceExtractor> resourceExtractor,
    LazySvc<RequirementsService> requirements,
    IModuleService moduleService)
    : ServiceBase(SxcLogName + "PgChSm", connect: [requirements, resourceExtractor, moduleService])
{
    /// <summary>
    /// Finalize the page and get all changes such as header modifications etc.
    /// </summary>
    /// <param name="moduleId">The module ID to check for any output caching instructions; use `0` if it should be ignored.</param>
    /// <param name="pss"></param>
    /// <param name="specs"></param>
    /// <param name="enableEdit"></param>
    /// <returns></returns>
    public RenderResult FinalizeAndGetAllChanges(int moduleId, IPageServiceShared pss, RenderSpecs specs, bool enableEdit)
    {
        var l = Log.Fn<RenderResult>(timer: true);
        if (enableEdit)
        {
            pss.Activate(SxcPageFeatures.ToolbarsInternal.NameId);
            pss.Activate(SxcPageFeatures.ToolbarsAutoInternal.NameId);
        }

        var assets = pss.GetAssetsAndFlush();
        var newPageFeaturesFromSettings = ((PageFeatures.PageFeatures)pss.PageFeatures).FeaturesFromSettingsGetNew(specs, Log);
        var (newAssets, rest) = ConvertSettingsAssetsIntoReal(newPageFeaturesFromSettings, specs);

        assets.AddRange(newAssets);
        assets = [.. assets.OrderBy(a => a.PosInPage)];

        // Collect Warnings of page features which may require other features enabled
        var features = pss.PageFeatures.GetFeaturesWithDependentsAndFlush(Log);

        var errors = requirements.Value
            .Check(features)
            .Select(f => f.Message)
            .ToList();

        // New beta 2025-03-18 v19.03.03
        var cacheSettings = moduleId != 0 ? ((ModuleService)moduleService).GetOutputCache(moduleId) : null;

        var csp = ((IPageServiceSharedInternal)pss).Csp;
        var result = new RenderResult
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
            CspEnabled = csp.IsEnabled,
            CspEnforced = csp.IsEnforced,
            CspParameters = csp.CspParameters(),
            Errors = errors,

            // New 19.03.03
            ModuleId = moduleId,                    // ModuleId for caching
            OutputCacheSettings = cacheSettings,    // Additional output-cache settings (often null)
        };

        // Whitelist any assets which were officially ok, or which were from the settings
        var additionalCsp = GetCspListFromAssets(assets);
        if (additionalCsp != null)
            result.CspParameters.Add(additionalCsp);

        return l.Return(result);
    }



    private (List<ClientAsset> newAssets, List<IPageFeature> rest) ConvertSettingsAssetsIntoReal(List<PageFeatureFromSettings> featuresFromSettings, RenderSpecs specs)
    {
        var l = Log.Fn<(List<ClientAsset> newAssets, List<IPageFeature> rest)>($"{featuresFromSettings.Count}");
        var newAssets = new List<ClientAsset>();
        foreach (var settingFeature in featuresFromSettings)
        {
            var autoOpt = settingFeature.AutoOptimize;
            var extracted = resourceExtractor.Value.Process(
                settingFeature.Html ?? "",
                new(
                    css: new(autoOpt, AddToBottom, CssDefaultPriority, false, false),
                    js: new(autoOpt, AddToBottom, JsDefaultPriority, autoOpt, autoOpt)
                )
            );
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
            // older code till 2025-03-17, not functional
            //extracted.Assets.ForEach(a => a.WhitelistInCsp = true);
            var assetsWithWhitelisting = extracted.Assets
                .Select(a => a with { WhitelistInCsp = true });

            newAssets.AddRange(assetsWithWhitelisting);

            // Reset the HTML to what's left after extracting the resources, except for Oqtane where we will keep it all
            if (!specs.IncludeAllAssetsInOqtane)
                settingFeature.Html = extracted.Html;
        }

        var featsLeft = featuresFromSettings
            .Where(f => !string.IsNullOrWhiteSpace(f.Html))
            .Cast<IPageFeature>()
            .ToList();

        return l.Return((newAssets, featsLeft), $"New: {newAssets.Count}; Rest: {featsLeft.Count}");
    }

    private static CspParameters? GetCspListFromAssets(IReadOnlyCollection<ClientAsset>? assets)
    {
        if (assets == null || assets.Count == 0)
            return null;
        var toWhitelist = assets
            .Where(a => a.WhitelistInCsp)
            .Where(a => !a.Url.NeverNull().StartsWith("/")) // skip local files
            .ToList();
        if (!toWhitelist.Any())
            return null;
        var whitelist = new CspParameters();
        foreach (var asset in toWhitelist)
            whitelist.Add((asset.IsJs ? "script" : "style") + "-src", asset.Url);

        return whitelist;
    }

}