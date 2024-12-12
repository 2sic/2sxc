using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.Internal.ClientAssets;

namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageServiceShared
{
    /// <summary>
    /// Assets consolidated from all render-results 
    /// </summary>
    private List<IClientAsset> Assets { get; } = [];

    public List<IClientAsset> GetAssetsAndFlush()
    {
        var assets = Assets.ToArray().ToList();
        Assets.Clear();
        return assets;
    }

    public void AddAssets(RenderEngineResult result)
    {
        if (result?.Assets == null) return;
        if (!result.Assets.Any()) return;
        Assets.AddRange(result.Assets);
    }

}