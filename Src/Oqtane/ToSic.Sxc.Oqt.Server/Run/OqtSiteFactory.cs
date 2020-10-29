using Oqtane.Repository;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class OqtSiteFactory
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtSiteFactory(ISiteRepository siteRepository) => _siteRepository = siteRepository;

        private readonly ISiteRepository _siteRepository;

        public ISite GetSite(int siteId) => new OqtSite(_siteRepository.GetSite(siteId));
    }
}
