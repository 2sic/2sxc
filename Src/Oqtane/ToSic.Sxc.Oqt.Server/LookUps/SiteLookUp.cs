using System;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class SiteLookUp : LookUpBase
    {
        public SiteState SiteState { get; }
        protected Oqtane.Models.Site Site { get; set; }
        private readonly Lazy<SiteStateInitializer> _siteStateInitializer;
        private readonly Lazy<SiteRepository> _siteRepository;

        public SiteLookUp(Lazy<SiteStateInitializer> siteStateInitializer, SiteState siteState, Lazy<SiteRepository> siteRepository)
        {
            Name = "Site";
            SiteState = siteState;
            _siteStateInitializer = siteStateInitializer;
            _siteRepository = siteRepository;
        }

        public Oqtane.Models.Site GetSource()
        {
            if (!_siteStateInitializer.Value.InitIfEmpty()) return null;
            var site = _siteRepository.Value.GetSite(SiteState.Alias.SiteId);
            //var oqtSite = _serviceProvider.Build<OqtSite>().Init(site);

            return site;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Site ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Site.SiteId}",
                    "guid" => $"{Site.SiteGuid}",
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