using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using Oqtane.Repository;

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    /// <summary>
    /// This is a Mvc implementation of a Tenant-object. 
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class OqtaneSiteFactory
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>
        public OqtaneSiteFactory(ISiteRepository siteRepository) => _siteRepository = siteRepository;

        private readonly ISiteRepository _siteRepository;

        public ITenant GetSite(int siteId) => new OqtaneTenantSite(_siteRepository.GetSite(siteId));
    }
}
