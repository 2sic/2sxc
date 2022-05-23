using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        public IList<HeadChange> Headers { get; } = new List<HeadChange>();
        public IList<HeadChange> GetHeadChangesAndFlush(ILog log)
        {
            var wrapLog = log.Fn<IList<HeadChange>>();
            var changes = Headers.ToArray().ToList();
            Headers.Clear();
            return wrapLog.Return(changes, $"{changes.Count}");
        }


        internal void Add(TagBase tag, string identifier = null)
        {
            if (tag == null) return;
            Headers.Add(new HeadChange { ChangeMode = GetMode(PageChangeModes.Append), Tag = tag, ReplacementIdentifier = identifier });
        }

    }
}
