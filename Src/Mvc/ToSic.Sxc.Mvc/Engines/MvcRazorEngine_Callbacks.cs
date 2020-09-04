using System;
using System.Collections.Generic;
using ToSic.Eav.Run;
using ToSic.Sxc.Search;

namespace ToSic.Sxc.Mvc.Engines
{
    public partial class MvcRazorEngine
    {
        /// <inheritdoc />
        public override void CustomizeData()
        {
            // TODO
            // (Webpage as IRazorComponent)?.CustomizeData();
        }

        /// <inheritdoc />
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo, DateTime beginDate)
        {
            //if (Webpage == null || searchInfos == null || searchInfos.Count <= 0) return;

            // call new signature
            // TODO
            // Webpage?.CustomizeSearch(searchInfos, moduleInfo, beginDate);
        }
    }
}
