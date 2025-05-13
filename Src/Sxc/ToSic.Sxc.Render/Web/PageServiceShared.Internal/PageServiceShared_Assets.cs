using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.Internal.ClientAssets;

namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageServiceShared : IPageServiceShared
{
    /// <summary>
    /// Assets consolidated from all render-results 
    /// </summary>
    private List<ClientAsset> Assets { get; } = [];

    public List<ClientAsset> GetAssetsAndFlush()
    {
        var assets = new List<ClientAsset>(Assets);
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