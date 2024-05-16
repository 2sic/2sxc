namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageServiceShared
{
    internal IList<PagePropertyChange> PropertyChanges { get; } = new List<PagePropertyChange>();

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
    internal string Queue(PageProperties property, string value, PageChangeModes change, string token)
    {
        PropertyChanges.Add(new()
        {
            ChangeMode = GetMode(change),
            Property = property,
            Value = value,
            ReplacementIdentifier = token,
        });
        return "";
    }

}