using System.Collections.Generic;
using System.Linq;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output
{
    public partial class OqtPageOutput
    {
        /// <summary>
        /// The JavaScript and Style assets
        /// from razor template and manual features
        /// </summary>
        /// <returns></returns>
        public List<SxcResource> GetSxcResources()
        {
            // assets from razor template
            var resources = SxcResourcesBuilder(RenderResult.Assets);
            // assets from manual features
            resources.AddRange(SxcResourcesBuilder(GetAssetsFromManualFeatures(RenderResult.ManualChanges)));
            return resources;
        }

        private static List<SxcResource> SxcResourcesBuilder(List<ClientAssetInfo> assets)
        {
            var resources = assets.Select(a => new SxcResource
            {
                ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
                Url = a.Url,
                IsExternal = a.IsExternal,
                Content = a.Content,
                UniqueId = a.Id
            }).ToList();
            return resources;
        }

        private List<ClientAssetInfo> GetAssetsFromManualFeatures(IList<IPageFeature> manualFeatures)
        {
            var assets = new List<ClientAssetInfo>();
            foreach (var manualFeature in manualFeatures)
            {
                // process manual features to get assets
                _oqtBlockResourceExtractor.Process(manualFeature.Html);
                assets.AddRange(_oqtBlockResourceExtractor.Assets);
            }
            return assets;
        }
    }
}