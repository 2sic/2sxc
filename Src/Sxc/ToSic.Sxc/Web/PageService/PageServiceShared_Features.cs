using System.Collections.Generic;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        public IEnumerable<string> Activate(params string[] keys) => PageFeatures.Activate(keys);


    }
}
