using System;
using System.Collections.Generic;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;

namespace ToSic.Sxc.Razor
{
    public partial class RazorEngine
    {
        /// <inheritdoc />
        public override void CustomizeData()
        {
            // TODO
            // (Webpage as IRazorComponent)?.CustomizeData();
        }

        /// <inheritdoc />
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
        {
            //if (Webpage == null || searchInfos == null || searchInfos.Count <= 0) return;

            // call new signature
            // TODO
            // Webpage?.CustomizeSearch(searchInfos, moduleInfo, beginDate);
        }
    }
}
