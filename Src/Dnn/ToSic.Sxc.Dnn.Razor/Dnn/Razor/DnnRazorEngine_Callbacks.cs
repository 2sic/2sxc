using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Razor;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.DataSources.Internal.Compatibility;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Search;
#pragma warning disable CS0618 // Type or member is obsolete

namespace ToSic.Sxc.Dnn.Razor;

partial class DnnRazorEngine
{
    /// <inheritdoc />
    public void Init(IBlock block, Purpose purpose) => Log.Do($"{nameof(purpose)}:{purpose}", () =>
    {
        Purpose = purpose;
        Init(block);
    });

    public override RenderEngineResult Render(object data)
    {
        var l = Log.Fn<RenderEngineResult>();

        // call engine internal feature to optionally change what data is actually used or prepared for search...
#pragma warning disable CS0618
        CustomizeData();
#pragma warning restore CS0618
        return l.Return(base.Render(data));
    }

    protected Purpose Purpose = Purpose.WebView;


    /// <inheritdoc />
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    public void CustomizeData()
    {
        if (Webpage is not IDnnRazorCustomize old) return;
        if (old.Data is not IBlockDataSource) return;
        old.CustomizeData();
    }

    /// <inheritdoc />
    [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
    public void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
    {
        if (Webpage == null || searchInfos == null || searchInfos.Count <= 0) return;

        // call new signature
        (Webpage as RazorComponent)?.CustomizeSearch(searchInfos, moduleInfo, beginDate);

        // also call old signature
        if (!(Webpage is SexyContentWebPage asWebPage)) return;
        var oldSignature = searchInfos.ToDictionary(si => si.Key, si => si.Value.Cast<ISearchInfo>().ToList());
        asWebPage.CustomizeSearch(oldSignature, ((Module<ModuleInfo>)moduleInfo).GetContents(), beginDate);
        searchInfos.Clear();
        foreach (var item in oldSignature)
            searchInfos.Add(item.Key, item.Value.Cast<ISearchItem>().ToList());
    }

}