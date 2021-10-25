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
            // Not implemented in v12
        }

        /// <inheritdoc />
        public override void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
        {
            // Not implemented in v12
        }
    }
}
