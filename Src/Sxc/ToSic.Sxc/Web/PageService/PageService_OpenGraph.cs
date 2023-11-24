using ToSic.Razor.Html5;

namespace ToSic.Sxc.Web.PageService;

public partial class PageService
{
    /// <inheritdoc />
    public string AddOpenGraph(string property, string content) => AddToHead(new MetaOg(property, content));

}