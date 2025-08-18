using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

partial class PageServiceShared
{
    /// <summary>
    /// Must be a real List, since it will be modified.
    /// </summary>
    internal List<PagePropertyChange> PropertyChanges { get; } = [];

    public IList<PagePropertyChange> GetPropertyChangesAndFlush(ILog log)
    {
        var l = log.Fn<IList<PagePropertyChange>>();
        var changes = PropertyChanges.ToArray().ToList();
        PropertyChanges.Clear();
        return l.Return(changes, $"{changes.Count}");
    }

    /// <summary>
    /// Add something to the queue for setting a page property
    /// </summary>
    public PagePropertyChange Queue(PageProperties property, string? value, PageChangeModes change, string? token)
    {
        var toAdd = new PagePropertyChange
        {
            ChangeMode = GetMode(change),
            Property = property,
            Value = value,
            ReplacementIdentifier = token,
        };
        PropertyChanges.Add(toAdd);
        return toAdd;
    }

}