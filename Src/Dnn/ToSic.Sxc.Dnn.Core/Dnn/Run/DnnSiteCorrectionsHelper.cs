using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Integration;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Dnn.Run;

/// <summary>
/// Special replacement of the FileSystemLoader - makes sure that unreliable Dnn initialization happens if necessary
/// </summary>
internal class DnnSiteCorrectionsHelper(IZoneMapper zoneMapper, ILog parentLog): HelperBase(parentLog, "Dnn.SiteCr")
{
    /// <summary>
    /// Special workaround for DNN because the site information is often incomplete (buggy)
    /// </summary>
    /// <returns></returns>
    internal ISite EnsureDnnSiteIsLoadedWhenDiFails(ISite siteFromDi, IAppIdentity appIdentity)
    {
        var l = Log.Fn<ISite>($"Trying to build path based on tenant. If it's in search mode, the {nameof(ISite)} would be {Eav.Constants.NullId}. Id: {siteFromDi.Id}");
        try
        {
            if (siteFromDi.Id != Eav.Constants.NullId)
                return l.Return(siteFromDi, $"All ok since siteId isn't {Eav.Constants.NullId}");

            l.A($"SiteId = {siteFromDi.Id} - not found. Must be in search mode as DI failed, will try to find correct PortalSettings");
            var correctedSite = zoneMapper.SiteOfApp(appIdentity.AppId);
            return l.Return(correctedSite, $"SiteId: {correctedSite.Id}");
        }
        catch (Exception e)
        {
            // ignore
            l.Ex(e);
            return l.Return(siteFromDi,"error");
        }
    }

}