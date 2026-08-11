using ToSic.Sxc.Web.Sys.ClientAssets;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

partial class PageServiceShared
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

    public void AddAssets(IList<ClientAsset>? result)
    {
        if (result.SafeNone())
            return;
        Assets.AddRange(result);
    }

}