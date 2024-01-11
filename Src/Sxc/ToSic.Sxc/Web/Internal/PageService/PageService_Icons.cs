using ToSic.Razor.Html5;
using ToSic.Razor.Internals.Page;

namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageService
{
    /// <inheritdoc />
    public string AddIcon(string path,
        NoParamOrder noParamOrder = default,
        string rel = "",
        int size = 0, string type = null)
    {
        AddToHead(new Icon(path, rel, size, type));
        return "";
    }

    /// <inheritdoc />
    public string AddIconSet(string path,
        NoParamOrder noParamOrder = default,
        object favicon = null, IEnumerable<string> rels = null, IEnumerable<int> sizes = null)
    {
        foreach (var s in IconSet.GenerateIconSet(path, favicon, rels, sizes))
            AddToHead(s);
        return "";
    }

}