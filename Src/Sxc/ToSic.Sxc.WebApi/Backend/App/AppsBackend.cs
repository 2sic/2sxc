using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Web.Internal.LightSpeed;

namespace ToSic.Sxc.Backend.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppsBackend(
    WorkApps workApps,
    IContextOfSite context,
    CodeInfoStats codeStats,
    Generator<IAppPathsMicroSvc> appPathsGen,
    LazySvc<GlobalPaths> globalPaths)
    : ServiceBase("Bck.Apps", connect: [workApps, codeStats, context, appPathsGen, globalPaths])
{
    public List<AppDto> Apps()
    {
        var list = workApps.GetApps(context.Site);
        return list.Select(CreateAppDto).ToList();
    }

    public List<AppDto> GetInheritableApps()
    {
        var list = workApps.GetInheritableApps(context.Site);
        return list.Select(CreateAppDto).ToList();
    }

    private AppDto CreateAppDto(IAppReader appReader)
    {
        AppMetadataDto lightspeed = null;
        var lightSpeedDeco = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader, Log);
        if (lightSpeedDeco.Entity != null)
            lightspeed = new () { Id = lightSpeedDeco.Id, Title = lightSpeedDeco.Title, IsEnabled = lightSpeedDeco.IsEnabled };

        var paths = appPathsGen.New().Get(appReader, context.Site);
        var thumbnail = AppAssetThumbnail.GetUrl(appReader, paths, globalPaths);
        var specs = appReader.Specs;
        return new ()
        {
            Id = appReader.AppId,
            IsApp = specs.NameId != Eav.Constants.DefaultAppGuid &&
                    specs.NameId != Eav.Constants.PrimaryAppGuid, // #SiteApp v13
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