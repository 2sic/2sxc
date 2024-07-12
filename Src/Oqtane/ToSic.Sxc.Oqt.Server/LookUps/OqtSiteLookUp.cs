using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Server.Plumbing;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Oqt.Server.LookUps;

internal class OqtSiteLookUp(LazySvc<AliasResolver> siteStateInitializer, SiteState siteState, LazySvc<SiteRepository> siteRepository) : LookUpBase(SourceSite, "LookUp in Oqtane Site")
{
    public SiteState SiteState { get; } = siteState;
    protected Oqtane.Models.Site Site { get; set; }

    public Oqtane.Models.Site GetSource()
    {
        if (!siteStateInitializer.Value.InitIfEmpty()) return null;
        var site = siteRepository.Value.GetSite(SiteState.Alias.SiteId);
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