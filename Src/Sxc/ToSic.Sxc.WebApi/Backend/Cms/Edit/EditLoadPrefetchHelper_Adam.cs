namespace ToSic.Sxc.Backend.Cms;

partial class EditLoadPrefetchHelper
{
    private Dictionary<string, Dictionary<string, IEnumerable< /*AdamItemDto*/object>>> PrefetchAdam(int appId, EditDto editData) => Log.Func(() =>
    {
        // Step 1: try to find hyperlink fields
        var bundlesHavingLinks = BundleWithLinkFields(editData, true);

        var bundlesWithAllKeys = bundlesHavingLinks.Select(set =>
        {
            var keys = new List<string>();
            var hKeys = set.HyperlinkFields?.Select(h => h.Key);
            if (hKeys != null) keys.AddRange(hKeys);
            var sKeys = set.StringFields?.Select(s => s.Key);
            if (sKeys != null) keys.AddRange(sKeys);
            return new
            {
                Set = set,
                Keys = keys
            };
        });

        var links = bundlesWithAllKeys
            .GroupBy(b => b.Set.Guid)
            .ToDictionary(
                b => b.Key.ToString(),
                b =>
                    b.SelectMany(selector: bundle => bundle.Keys.Select(key => new
                        {
                            Key = key,
                            Dic = GetAdamListOfItems(appId, bundle.Set, key),
                        }))
                        // skip empty bits to avoid UI from relying on these nodes to always exist
                        .Where(r => r.Dic.Any())
                        // Step 2: Check which ones have a link reference
                        .ToDictionary(r => r.Key, r => r.Dic)
            );

        return links;
    });

    private IEnumerable</*AdamItemDto*/object> GetAdamListOfItems(int appId, BundleWithLinkField set, string key) => Log.Func(() =>
    {
        var adamListMaker = adamTransGetItems.New();
        adamListMaker.Init(appId, set.ContentTypeName, set.Guid, key, false);
        var list = adamListMaker.ItemsInField(string.Empty, false) as IEnumerable<AdamItemDto>;
        return list;
    });
}