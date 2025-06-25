using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Render.Sys.Specs;

#pragma warning disable CS0618 // Type or member is obsolete

namespace ToSic.Sxc.Dnn.Razor;

partial class DnnRazorEngine
{
    /// <inheritdoc />
    public void Init(IBlock block, Purpose purpose)
    {
        // #RemovedV20 #ModulePublish
        var l = Log.Fn($"{nameof(purpose)}:{purpose}");
        //Purpose = purpose;
        Init(block);
        l.Done();
    }

    public override RenderEngineResult Render(RenderSpecs specs)
    {
        var l = Log.Fn<RenderEngineResult>();

        // #RemovedV20 #ModulePublish
        // 2025-06 removed for v20
//        // call engine internal feature to optionally change what data is actually used or prepared for search...
//#pragma warning disable CS0618
//        CustomizeData();
//#pragma warning restore CS0618
        return l.Return(base.Render(specs));
    }

    // #RemovedV20 #ModulePublish
    //#pragma warning disable CS0618 // Type or member is obsolete
    //    protected Purpose Purpose = Purpose.WebView;
    //#pragma warning restore CS0618 // Type or member is obsolete

    // #RemovedV20 #ModulePublish
    ///// <inheritdoc />
    //[Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    //public void CustomizeData()
    //{
    //    if (EntryRazorComponent is not IDnnRazorCustomize old) return;
    //    if (old.Data is not IBlockDataSource) return;
    //    old.CustomizeData();
    //}

    // #RemovedV20 #ModulePublish
    ///// <inheritdoc />
    //[Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    //public void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
    //{
    //    if (EntryRazorComponent == null || searchInfos == null || searchInfos.Count <= 0) return;

    //    // call new signature
    //    (EntryRazorComponent as RazorComponent)?.CustomizeSearch(searchInfos, moduleInfo, beginDate);

    //    // also call old signature
    //    if (EntryRazorComponent is not SexyContentWebPage asWebPage) return;
    //    var oldSignature = searchInfos.ToDictionary(si => si.Key, si => si.Value.Cast<ISearchInfo>().ToList());
    //    asWebPage.CustomizeSearch(oldSignature, ((Module<ModuleInfo>)moduleInfo).GetContents(), beginDate);
    //    searchInfos.Clear();
    //    foreach (var item in oldSignature)
    //        searchInfos.Add(item.Key, item.Value.Cast<ISearchItem>().ToList());
    //}

}