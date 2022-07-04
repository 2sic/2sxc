using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.PageFeatures;
using BuiltInFeatures = ToSic.Sxc.Web.PageFeatures.BuiltInFeatures;

namespace ToSic.Sxc.Web.PageService
{
    /// <summary>
    /// This should bundle all the page changes once a module is done.
    /// Usually used at the top-level of render-result, and in future also on page-level dynamic code
    /// </summary>
    public class PageChangeSummary: HasLog
    {

        public PageChangeSummary(
            LazyInitLog<IBlockResourceExtractor> resourceExtractor,
            LazyInitLog<RequirementsService> requirements
        ) : base(Constants.SxcLogName + "PgChSm")
        {
            _requirements = requirements.SetLog(Log);
            _resourceExtractor = resourceExtractor.SetLog(Log);
        }

        private readonly LazyInitLog<IBlockResourceExtractor> _resourceExtractor;
        private readonly LazyInitLog<RequirementsService> _requirements;

        public IRenderResult FinalizeAndGetAllChanges(PageServiceShared pss, bool enableEdit)
        {
            var callLog = Log.Fn<IRenderResult>();

            if (enableEdit)
            {
                pss.Activate(BuiltInFeatures.ToolbarsInternal.NameId);
                pss.Activate(BuiltInFeatures.ToolbarsAutoInternal.NameId);
            }

            var assets = pss.Assets;
            var (newAssets, rest) = ConvertSettingsAssetsIntoReal(pss.PageFeatures.FeaturesFromSettingsGetNew(Log));

            assets.AddRange(newAssets);
            assets = assets.OrderBy(a => a.PosInPage).ToList();

            // TODO: Collect Warnings
            var features = pss.PageFeatures.GetFeaturesWithDependentsAndFlush(Log);

            var errors = _requirements.Ready
                .Check(features)
                .Select(f => f.Message)
                .ToList();

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
                CspEnabled = pss.Csp.IsEnabled,
                CspEnforced = pss.Csp.IsEnforced,
                CspParameters = pss.Csp.CspParameters(),
                Errors = errors,
            };

            // Whitelist any assets which were officially ok, or which were from the settings
            var additionalCsp = GetCspListFromAssets(assets);
            if (additionalCsp != null) result.CspParameters.Add(additionalCsp);

            return callLog.Return(result);
        }


        private (List<IClientAsset> newAssets, List<IPageFeature> rest) ConvertSettingsAssetsIntoReal(List<PageFeatureFromSettings> featuresFromSettings)
        {
            var wrapLog = Log.Fn<(List<IClientAsset> newAssets, List<IPageFeature> rest)>($"{featuresFromSettings.Count}");
            var newAssets = new List<IClientAsset>();
            foreach (var settingFeature in featuresFromSettings)
            {
                var extracted = _resourceExtractor.Ready.Process(settingFeature.Html);
                if (!extracted.Assets.Any()) continue;
                Log.A($"Moved Feature Html {settingFeature.NameId} to assets");

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

            return wrapLog.Return((newAssets, featsLeft), $"New: {newAssets.Count}; Rest: {featsLeft.Count}");
        }

        private CspParameters GetCspListFromAssets(List<IClientAsset> assets)
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
}
