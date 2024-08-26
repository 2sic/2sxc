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
    public DnnAppFileSystemLoader(LazySvc<IZoneMapper> zoneMapper, MyServices services): base(services, "Dnn.AppStf")
    {
        ConnectLogs([
            _zoneMapper = zoneMapper
        ]);
    }

    private readonly LazySvc<IZoneMapper> _zoneMapper;

    protected override ISite Site => _site
        ??= new DnnSiteCorrectionsHelper(_zoneMapper.Value, Log)
            .EnsureDnnSiteIsLoadedWhenDiFails(Services.Site, AppIdentity);
    private ISite _site;
}