using ToSic.Razor.Blade;

namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageServiceShared
{
    public IList<HeadChange> Headers { get; } = new List<HeadChange>();
    public IList<HeadChange> GetHeadChangesAndFlush(ILog log)
    {
        var l = log.Fn<IList<HeadChange>>();
        var changes = Headers.ToArray().ToList();
        Headers.Clear();
        return l.Return(changes, $"{changes.Count}");
    }


    internal void Add(IHtmlTag tag, string identifier = null)
    {
        if (tag == null) return;
        Headers.Add(new() { ChangeMode = GetMode(PageChangeModes.Append), Tag = tag, ReplacementIdentifier = identifier });
    }

}