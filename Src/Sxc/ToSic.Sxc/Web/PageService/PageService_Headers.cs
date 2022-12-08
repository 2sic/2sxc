using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {

        /// <inheritdoc />
        public string AddToHead(TagBase tag)
        {
            PageServiceShared.Add(tag);
            return "";
        }

        /// <inheritdoc />
        public string AddToHead(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            AddToHead(Tag.Custom(html));
            return "";
        }

        /// <inheritdoc />
        public string AddMeta(string name, string content) => AddToHead(_htmlTagsLazy.Value.Meta().Name(name).Content(content));
    }
}
