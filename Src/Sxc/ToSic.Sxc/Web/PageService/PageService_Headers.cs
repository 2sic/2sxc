using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {

        /// <inheritdoc />
        public void AddToHead(TagBase tag) => PageServiceShared.Add(tag);

        /// <inheritdoc />
        public void AddToHead(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return;
            AddToHead(Tag.Custom(html));
        }

        /// <inheritdoc />
        public void AddMeta(string name, string content) => AddToHead(new Meta(name, content));
    }
}
