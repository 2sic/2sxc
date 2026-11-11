using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Sxc.Cms;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnSiteOfZoneSvc(LazySvc<DnnSiteFactory> siteFactory /* lazy is very important, otherwise cyclical logs */)
    : ServiceBase("DNN.ZoneMp", connect: [siteFactory]), ISiteOfZoneSvc
{

    public ISite SiteOfZone(int zoneId)
    {
        var l = Log.Fn<ISite>($"{zoneId}");
        var portalController = PortalController.Instance;
        var allPortals = portalController.GetPortals();
        l.A($"Sites/Portals Count: {allPortals.Count}");
        var found = allPortals
            .Cast<PortalInfo>()
            .Select(p =>
            {
                var pSettings = portalController.GetPortalSettings(((IPortalInfo)p).PortalId);
                if (!pSettings.TryGetValue(SiteSettingNames.SiteKeyForZoneId, out var portalZoneId))
                    return null;
                if (!int.TryParse(portalZoneId, out var zid))
                    return null;
                return zid == zoneId
                    ? new PortalSettings(p)
                    : null;
            })
            .FirstOrDefault(f => f != null);

        // 2026-10-18 2dm: must check the consequences of this
        // It may have had a side effect till now, updating the site
        // scoped TODO: #VerifySiteFactorySplitIsOk
        return found == null
            ? l.ReturnNull("not found")
            : l.Return(siteFactory.Value.CreateFromPortal(found), $"found {found.PortalId}");
    }

}
