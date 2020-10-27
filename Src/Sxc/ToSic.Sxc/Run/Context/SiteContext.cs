using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Run.Context
{
    /// <summary>
    /// The site where the current instance is running. 
    /// </summary>
    [WorkInProgressApi("Still WIP")]
    public class SiteContext
    {
        /// <summary>
        /// The site id - corresponds to a DNN PortalId or the Oqtane SiteId
        /// </summary>
        public int Id;
    }
}
