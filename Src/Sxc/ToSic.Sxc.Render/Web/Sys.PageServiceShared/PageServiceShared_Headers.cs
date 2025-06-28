using ToSic.Razor.Blade;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

partial class PageServiceShared
{
    /// <summary>
    /// Must be a real List, since it will be modified.
    /// </summary>
    public List<HeadChange> Headers { get; } = [];

    public IList<HeadChange> GetHeadChangesAndFlush(ILog log)
    {
        var l = log.Fn<IList<HeadChange>>();
        var changes = Headers.ToArray().ToList();
        Headers.Clear();
        return l.Return(changes, $"{changes.Count}");
    }


    public void Add(IHtmlTag? tag, string? identifier = null)
    {
        if (tag == null)
            return;
        Headers.Add(new()
        {
            ChangeMode = GetMode(PageChangeModes.Append),
            Tag = tag,
            ReplacementIdentifier = identifier,
        });
    }

}