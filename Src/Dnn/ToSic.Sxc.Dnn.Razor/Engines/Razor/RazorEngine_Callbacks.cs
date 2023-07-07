using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.SexyContent.Razor;
using ToSic.SexyContent.Search;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;

using ToSic.Sxc.Search;

namespace ToSic.Sxc.Engines
{
    public partial class RazorEngine
    {

        /// <inheritdoc />
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        public override void CustomizeData()
        {
            if (!(Webpage is IDnnRazorCustomize old)) return;
            if (!(old.Data is IBlockDataSourceOld isOld)) return;
            //(isOld as ContextData)?.ToggleOldMode();
            old.CustomizeData();
        }

        /// <inheritdoc />
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
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
}
