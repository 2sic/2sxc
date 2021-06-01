using ToSic.Razor.Html5;

namespace ToSic.Sxc.Web.PageService
{
    public partial class Page
    {
        /// <inheritdoc />
        public void AddOpenGraph(string property, string content) => AddToHead(new MetaOg(property, content));

    }
}
