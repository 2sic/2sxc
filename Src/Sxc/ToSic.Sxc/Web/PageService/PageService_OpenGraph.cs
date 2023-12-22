using ToSic.Razor.Html5;

namespace ToSic.Sxc.Web.PageService;

partial class PageService
{
    /// <inheritdoc />
    public string AddOpenGraph(string property, string content) => AddToHead(new MetaOg(property, content));

}