namespace ToSic.Sxc.Backend.Cms.Load.Activities;

partial class EditLoadActivityPrefetchHelper
{
    private Dictionary<string, Dictionary<string, IEnumerable< /*AdamItemDto*/object>>> PrefetchAdam(int appId, EditLoadDto editData)
    {
        var l = Log.Fn<Dictionary<string, Dictionary<string, IEnumerable<object>>>>();

        // Step 1: try to find hyperlink fields
        var bundlesHavingLinks = BundleWithLinkFields(editData, true);

        var bundlesWithAllKeys = bundlesHavingLinks
            .Select(set =>
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
            })
            .ToList();

        var links = bundlesWithAllKeys
            .GroupBy(b => b.Set.Guid)
            .ToDictionary(
                b => b.Key.ToString(),
                b => b
                    .SelectMany(selector: bundle => bundle.Keys
                        .Select(key => new
                        {
                            Key = key,
                            Dic = GetAdamListOfItems(appId, bundle.Set, key) as IEnumerable<object>,
                        })
                    )
                    // skip empty bits to avoid UI from relying on these nodes to always exist
                    .Where(r => r.Dic.Any())
                    // Step 2: Check which ones have a link reference
                    .ToDictionary(r => r.Key, r => r.Dic)
            );

        return l.Return(links);
    }

    private IEnumerable<AdamItemDto> GetAdamListOfItems(int appId, BundleWithLinkField set, string key)
    {
        var l = Log.Fn<IEnumerable<AdamItemDto>>();
        var adamListMaker = adamTransGetItems.New(new()
        {
            AppId = appId,
            ContentType = set.ContentTypeName,
            ItemGuid = set.Guid,
            Field = key,
            UsePortalRoot = false,
        });
        //adamListMaker.Setup(appId, set.ContentTypeName, set.Guid, key, false);
        var list = adamListMaker.GetAdamItemsForPrefetch(string.Empty, false);
        return l.Return(list);
    }
}