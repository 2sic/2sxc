using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Code.InfoSystem;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppsBackend(
    WorkApps workApps,
    IContextOfSite context,
    CodeInfoStats codeStats,
    Generator<IAppPathsMicroSvc> appPathsGen,
    LazySvc<GlobalPaths> globalPaths)
    : ServiceBase("Bck.Apps", connect: [workApps, codeStats, context, appPathsGen, globalPaths])
{
    public ICollection<AppDto> Apps()
    {
        var list = workApps.GetApps(context.Site);
        return list.Select(CreateAppDto).ToListOpt();
    }

    public ICollection<AppDto> GetInheritableApps()
    {
        var list = workApps.GetInheritableApps(context.Site);
        return list.Select(CreateAppDto).ToListOpt();
    }

    private AppDto CreateAppDto(IAppReader appReader)
    {
        AppMetadataDto? lightspeed = null;
        var lightSpeedDeco = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader/*, Log*/);
        if (lightSpeedDeco.Entity != null! /* paranoid */)
            lightspeed = new () { Id = lightSpeedDeco.Id, Title = lightSpeedDeco.Title, IsEnabled = lightSpeedDeco.IsEnabled };

        var paths = appPathsGen.New().Get(appReader, context.Site);
        var thumbnail = AppAssetThumbnail.GetUrl(appReader, paths, globalPaths);
        var specs = appReader.Specs;
        return new ()
        {
            Id = appReader.AppId,
            IsApp = specs.NameId != KnownAppsConstants.DefaultAppGuid &&
                    specs.NameId != KnownAppsConstants.PrimaryAppGuid, // #SiteApp v13
            Guid = specs.NameId,
            Name = specs.Name,
            Folder = specs.Folder,
            AppRoot = paths.Path,
            IsHidden = specs.Configuration.IsHidden,
            ConfigurationId = specs.Configuration.Id,
            Items = appReader.List.Count,
            Thumbnail = thumbnail,
            Version = specs.VersionSafe(),
            IsGlobal = appReader.IsShared(),
            IsInherited = appReader.IsInherited(),
            Lightspeed = lightspeed,
            HasCodeWarnings = codeStats.AppHasWarnings(appReader.AppId),
        };
    }
}