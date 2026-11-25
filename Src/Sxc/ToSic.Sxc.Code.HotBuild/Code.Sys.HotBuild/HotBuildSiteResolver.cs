using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Sys;

namespace ToSic.Sxc.Code.Sys.HotBuild;

internal static class HotBuildSiteResolver
{
    /// <summary>
    /// Resolve the site which actually owns the app files.
    /// This must not short-circuit on ZoneId, because platform can host shared apps across sites
    /// and the first cold request may come from a different site than owning site.
    /// </summary>
    internal static ISite ResolveForApp(ISite siteFromDi, IAppIdentity appIdentity, ISiteOfAppSvc siteOfAppSvc)
    {
        try
        {
            var mappedSite = siteOfAppSvc.SiteOfApp(appIdentity.AppId);
            if (mappedSite is not { Id: not EavConstants.NullId })
                return siteFromDi;

            // Reuse the DI site instance when it already points at the owning site,
            // otherwise switch to the mapped owner so AppCode/Dependencies resolve from the correct site.
            return mappedSite.Id == siteFromDi.Id
                ? siteFromDi
                : mappedSite;
        }
        catch
        {
            return siteFromDi;
        }
    }
}
