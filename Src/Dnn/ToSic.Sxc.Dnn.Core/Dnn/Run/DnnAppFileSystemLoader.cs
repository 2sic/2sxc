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
    /// Constructor for DI - you must always call Init(...) afterwards
    /// </summary>
    public DnnAppFileSystemLoader(IZoneMapper zoneMapper, MyServices services): base(services, "Dnn.AppStf")
    {
        ConnectServices(
            ZoneMapper = zoneMapper
        );
    }

    protected readonly IZoneMapper ZoneMapper;


    /// <summary>
    /// Init Path After AppId must be in an own method, as each implementation may have something custom to handle this
    /// </summary>
    /// <returns></returns>
    protected override bool InitPathAfterAppId()
    {
        var l = Log.Fn<bool>();
        try
        {
            Log.A($"Trying to build path based on tenant. If it's in search mode, the {nameof(ISite)} would be {Eav.Constants.NullId}. Id: {Site.Id}");
            EnsureDnnSiteIsLoadedWhenDiFails();
            base.InitPathAfterAppId();
            return l.ReturnTrue(Path);
        }
        catch (Exception e)
        {
            // ignore
            Log.Ex(e);
            return l.ReturnFalse("error");
        }
    }


    /// <summary>
    /// Special workaround for DNN because the site information is often incomplete (buggy)
    /// </summary>
    /// <returns></returns>
    private void EnsureDnnSiteIsLoadedWhenDiFails() => Log.Do(() =>
    {
        if (Site.Id != Eav.Constants.NullId/* && Site.Id != 0*/) // 2021-12-09 2dm disabled zero check, because portal 0 is actually very common
            return $"All ok since siteId isn't {Eav.Constants.NullId}";
        Log.A($"SiteId = {Site.Id} - not found. Must be in search mode or something else DI-style failed, will try to find correct PortalSettings");
        Site = ZoneMapper.SiteOfApp(AppIdentity.AppId);
        return $"SiteId: {Site.Id}";
    });

}