using System.Collections.Generic;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Web.PageService
{
    public interface IChangeQueue
    {
        //IList<PagePropertyChange> PropertyChanges { get; }

        /// <summary>
        /// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
        /// </summary>
        /// <returns></returns>
        IList<PagePropertyChange> GetPropertyChangesAndFlush();
        
        //IList<HeadChange> Headers { get; }
        
        /// <summary>
        /// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
        /// </summary>
        /// <returns></returns>
        IList<HeadChange> GetHeadChangesAndFlush();

    }

}
