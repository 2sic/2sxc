using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Web.PageService;

public partial class PageServiceShared
{
    internal IList<PagePropertyChange> PropertyChanges { get; } = new List<PagePropertyChange>();

    public IList<PagePropertyChange> GetPropertyChangesAndFlush(ILog log)
    {
        var wrapLog = log.Fn<IList<PagePropertyChange>>();
        var changes = PropertyChanges.ToArray().ToList();
        PropertyChanges.Clear();
        return wrapLog.Return(changes, $"{changes.Count}");
    }

    /// <summary>
    /// Add something to the queue for setting a page property
    /// </summary>
    internal string Queue(PageProperties property, string value, PageChangeModes change, string token)
    {
        PropertyChanges.Add(new PagePropertyChange
        {
            ChangeMode = GetMode(change),
            Property = property,
            Value = value,
            ReplacementIdentifier = token,
        });
        return "";
    }

}