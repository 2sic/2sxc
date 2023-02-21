using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.PageFeatures;
using BuiltInFeatures = ToSic.Sxc.Web.PageFeatures.BuiltInFeatures;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.Web.PageService
{
    /// <summary>
    /// This should bundle all the page changes once a module is done.
    /// Usually used at the top-level of render-result, and in future also on page-level dynamic code
    /// </summary>
    public class PageChangeSummary: ServiceBase
    {

        public PageChangeSummary(
            LazySvc<IBlockResourceExtractor> resourceExtractor,
            LazySvc<RequirementsService> requirements
        ) : base(Constants.SxcLogName + "PgChSm")
        {
            ConnectServices(
                _requirements = requirements,
                _resourceExtractor = resourceExtractor
            );
        }

        private readonly LazySvc<IBlockResourceExtractor> _resourceExtractor;
        private readonly LazySvc<RequirementsService> _requirements;

        public IRenderResult FinalizeAndGetAllChanges(PageServiceShared pss, bool enableEdit) => Log.Func(timer: true, func: () =>
        {
            if (enableEdit)
            {
                pss.Activate(BuiltInFeatures.ToolbarsInternal.NameId);
                pss.Activate(BuiltInFeatures.ToolbarsAutoInternal.NameId);
            }

            var assets = pss.Assets;
            var (newAssets, rest) = ConvertSettingsAssetsIntoReal(pss.PageFeatures.FeaturesFromSettingsGetNew(Log));

            assets.AddRange(newAssets);
            assets = assets.OrderBy(a => a.PosInPage).ToList();

            // Collect Warnings of page features which may require other features enabled
            var features = pss.PageFeatures.GetFeaturesWithDependentsAndFlush(Log);

            var errors = _requirements.Value
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

            return result;
        });


        private (List<IClientAsset> newAssets, List<IPageFeature> rest) ConvertSettingsAssetsIntoReal(
            List<PageFeatureFromSettings> featuresFromSettings
        ) => Log.Func($"{featuresFromSettings.Count}", l =>
        {
            var newAssets = new List<IClientAsset>();
            foreach (var settingFeature in featuresFromSettings)
            {
                var extracted = _resourceExtractor.Value.Process(settingFeature.Html);
                if (!extracted.Assets.Any()) continue;
                l.A($"Moved Feature Html {settingFeature.NameId} to assets");

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

            return ((newAssets, featsLeft), $"New: {newAssets.Count}; Rest: {featsLeft.Count}");
        });

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
