using System.Collections.Generic;

namespace ToSic.Sxc.Web.PageService
{
    public interface IChangeQueue
    {
        IList<PagePropertyChange> PropertyChanges { get; }
        IList<HeadChange> Headers { get; }
    }

}
