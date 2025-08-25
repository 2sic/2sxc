using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Page.Sys;

partial class PageService
{

    /// <inheritdoc />
    public string AddToHead(IHtmlTag tag)
    {
        var added = PageServiceShared.Add(tag);
        if (added != null)
            Listeners.AddToHead(added.Value); // no duplicates, because this is a new tag
        return "";
    }

    /// <inheritdoc />
    public string AddToHead(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return "";
        AddToHead(Tag.Custom(html));
        return "";
    }

    /// <inheritdoc />
    public string AddMeta(string name, string content)
        => AddToHead(htmlTagsLazy.Value.Meta().Name(name).Content(content));
}