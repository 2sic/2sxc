using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.PageService
{
    public partial class Page
    {
        public IList<HeadChange> Headers { get; } = new List<HeadChange>();
        public IList<HeadChange> GetHeadChangesAndFlush()
        {
            var changes = Headers.ToArray().ToList();
            Headers.Clear();
            return changes;
        }

        /// <inheritdoc />
        public void AddToHead(TagBase tag) => Add(tag);

        /// <inheritdoc />
        public void AddToHead(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return;
            AddToHead(Tag.Custom(html));
        }

        /// <inheritdoc />
        public void AddMeta(string name, string content) => AddToHead(new Meta(name, content));

        private void Add(TagBase tag, string identifier = null)
        {
            if (tag == null) return;
            Headers.Add(new HeadChange {ChangeMode = GetMode(PageChangeModes.Append), Tag = tag, ReplacementIdentifier = identifier});
        }
    }
}
