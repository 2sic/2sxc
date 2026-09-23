using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.WebApi.Sys.App.Details;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Apps.Sys.DataSources;

/// <summary>
/// Extension of the original IAppDetailsService - adding thumbnail and lightspeed information.
///
/// It's used by data sources higher up, but certain information isn't available till here.
/// </summary>
/// <param name="baseSvc"></param>
/// <param name="appPathsGen"></param>
/// <param name="globalPaths"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AppDetailsServiceExtended(AppDetailsService baseSvc, Generator<IAppPathsMicroSvc> appPathsGen, LazySvc<GlobalPaths> globalPaths)
    : ServiceBase("App.DetSvc",  connect: [baseSvc, appPathsGen, globalPaths]), IAppDetailsService
{
    public AppDetailsRaw GetDetails(ISite site, IAppReader appReader)
    {
        var fromBase = baseSvc.GetDetails(site, appReader);

        var lightSpeedDeco = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader/*, Log*/);
        AppMetadataDto? lightspeed = (lightSpeedDeco as ICanBeEntity)?.Entity == null! /* paranoid */
            ? null
            : new()
            {
                Id = lightSpeedDeco.Id,
                Title = lightSpeedDeco.Title,
                IsEnabled = lightSpeedDeco.IsEnabled
            };

        var paths = appPathsGen.New().Get(appReader, site);
        var thumbnail = AppAssetThumbnail.GetUrl(appReader, paths, globalPaths);
        return fromBase with
        {
            Thumbnail = thumbnail,
            LightSpeed = lightspeed,
        };

    }
}