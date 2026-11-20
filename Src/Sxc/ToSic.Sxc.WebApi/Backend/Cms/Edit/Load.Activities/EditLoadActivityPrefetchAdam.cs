using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Eav.Ui.Edit.Sys.Load.Prefetch;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditLoadActivityPrefetchAdam(Generator<IAdamPrefetchHelper, AdamContext.Options> adamTransGetItems, IContextCompleteService ctxService)
    : ServiceBase(SxcLogName + ".Prefetch", connect: [adamTransGetItems, ctxService]),
        IEditLoadPrefetchProvider
{
    public const string NameId = "Adam";

    public Task<Package<object>> Handle(WorkContext mainCtx, Package<EditLoadDto> package)
    {
        var result = PrefetchAdam(package.Data);
        return Task.FromResult(package.RePackage(result as object));
    }

    public Dictionary<string, Dictionary<string, IEnumerable< /*AdamItemDto*/object>>> PrefetchAdam(EditLoadDto editData)
    {
        var l = Log.Fn<Dictionary<string, Dictionary<string, IEnumerable<object>>>>();

        // Step 1: try to find hyperlink fields
        var bundlesHavingLinks = BundleWithLinkField.ExtractLinkFields(editData, true);

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
                            Dic = GetAdamListOfItems(bundle.Set, key) as IEnumerable<object>,
                        })
                    )
                    // skip empty bits to avoid UI from relying on these nodes to always exist
                    .Where(r => r.Dic.Any())
                    // Step 2: Check which ones have a link reference
                    .ToDictionary(r => r.Key, r => r.Dic)
            );

        return l.Return(links);
    }

    private IEnumerable<AdamItemDto> GetAdamListOfItems(BundleWithLinkField set, string key)
    {
        var l = Log.Fn<IEnumerable<AdamItemDto>>();
        var adamListMaker = adamTransGetItems.New(new(
            Context: ctxService.AppContextRequired(),
            ContentType: set.ContentTypeName,
            EntityGuid: set.Guid,
            FieldName: key,
            UsePortalRoot: false
            ));
        var list = adamListMaker.GetAdamItemsForPrefetch(string.Empty, false);
        return l.Return(list);
    }


}
