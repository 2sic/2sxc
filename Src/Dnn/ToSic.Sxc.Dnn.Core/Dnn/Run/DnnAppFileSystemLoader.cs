using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Integration;

namespace ToSic.Sxc.Dnn.Run;

/// <summary>
/// Special replacement of the FileSystemLoader - makes sure that unreliable Dnn initialization happens if necessary
/// </summary>
internal class DnnAppFileSystemLoader : AppFileSystemLoader
{
    /// <summary>
    /// Constructor for DI - you must always call Init(...) afterward
    /// </summary>
    public DnnAppFileSystemLoader(IZoneMapper zoneMapper, MyServices services): base(services, "Dnn.AppStf")
    {
        ConnectLogs([
            ZoneMapper = zoneMapper
        ]);
    }

    protected readonly IZoneMapper ZoneMapper;

    protected override ISite Site => _site ??= EnsureDnnSiteIsLoadedWhenDiFails(Services.Site);
    private ISite _site;

    /// <summary>
    /// Special workaround for DNN because the site information is often incomplete (buggy)
    /// </summary>
    /// <returns></returns>
    private ISite EnsureDnnSiteIsLoadedWhenDiFails(ISite siteFromDi)
    {
        var l = Log.Fn<ISite>($"Trying to build path based on tenant. If it's in search mode, the {nameof(ISite)} would be {Eav.Constants.NullId}. Id: {siteFromDi.Id}");
        try
        {
            if (siteFromDi.Id != Eav.Constants.NullId)
                return l.Return(siteFromDi, $"All ok since siteId isn't {Eav.Constants.NullId}");

            l.A($"SiteId = {siteFromDi.Id} - not found. Must be in search mode as DI failed, will try to find correct PortalSettings");
            var correctedSite = ZoneMapper.SiteOfApp(AppIdentity.AppId);
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