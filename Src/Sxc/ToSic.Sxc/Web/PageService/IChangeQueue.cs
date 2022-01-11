using System.Collections.Generic;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.PageService
{
    public interface IChangeQueue
    {
        //IList<PagePropertyChange> PropertyChanges { get; }

        /// <summary>
        /// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
        /// </summary>
        /// <returns></returns>
        IList<PagePropertyChange> GetPropertyChangesAndFlush(ILog log);
        
        //IList<HeadChange> Headers { get; }
        
        /// <summary>
        /// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
        /// </summary>
        /// <returns></returns>
        IList<HeadChange> GetHeadChangesAndFlush(ILog log);


        int? HttpStatusCode { get; set; }
        string HttpStatusMessage { get; set; }

        IPageFeatures Features { get; }

    }
}
