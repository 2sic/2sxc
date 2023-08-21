using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Server.Plumbing;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class OqtSiteLookUp : LookUpBase
    {
        public SiteState SiteState { get; }
        protected Oqtane.Models.Site Site { get; set; }
        private readonly LazySvc<SiteStateInitializer> _siteStateInitializer;
        private readonly LazySvc<SiteRepository> _siteRepository;

        public OqtSiteLookUp(LazySvc<SiteStateInitializer> siteStateInitializer, SiteState siteState, LazySvc<SiteRepository> siteRepository)
        {
            Name = SourceSite;
            SiteState = siteState;
            _siteStateInitializer = siteStateInitializer;
            _siteRepository = siteRepository;
        }

        public Oqtane.Models.Site GetSource()
        {
            if (!_siteStateInitializer.Value.InitIfEmpty()) return null;
            var site = _siteRepository.Value.GetSite(SiteState.Alias.SiteId);
            return site;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Site ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    KeyId => $"{Site.SiteId}",
                    KeyGuid => $"{Site.SiteGuid}",
                    OldDnnSiteId => $"warning: you have requested '{OldDnnSiteId}' which doesn't work in hybrid/oqtane. Use {KeyId}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}