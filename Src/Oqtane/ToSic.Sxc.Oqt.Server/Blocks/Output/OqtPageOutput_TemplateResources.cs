using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

partial class OqtPageOutput
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
        resources.AddRange(SxcResourcesBuilder(GetAssetsFromManualFeatures(RenderResult.FeaturesFromSettings)));
        return resources;
    }

    private static List<SxcResource> SxcResourcesBuilder(IList<IClientAsset> assets)
    {
        var resources = assets.Select(a => new SxcResource
        {
            ResourceType = a.IsJs ? ResourceType.Script : ResourceType.Stylesheet,
            Url = a.Url,
            IsExternal = a.IsExternal,
            Content = a.Content,
            UniqueId = a.Id,
            HtmlAttributes = a.HtmlAttributes // will copy HtmlAttributes and also try to set Integrity and CrossOrigin properties
        }).ToList();
        return resources;
    }

    private List<IClientAsset> GetAssetsFromManualFeatures(IList<IPageFeature> manualFeatures)
    {
        var assets = new List<IClientAsset>();
        foreach (var manualFeature in manualFeatures)
        {
            // process manual features to get assets
            var result = blockResourceExtractor.Process(manualFeature.Html);
            assets.AddRange(result.Assets);
        }
        return assets;
    }
}