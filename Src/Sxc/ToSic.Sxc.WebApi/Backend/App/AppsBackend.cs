using ToSic.Eav.Apps.Integration;
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
        var lightSpeedDeco = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader.StateCache, Log);
        if (lightSpeedDeco.Entity != null)
            lightspeed = new () { Id = lightSpeedDeco.Id, Title = lightSpeedDeco.Title, IsEnabled = lightSpeedDeco.IsEnabled };

        var paths = appPathsGen.New().Init(context.Site, appReader);
        var thumbnail = AppAssetThumbnail.GetUrl(appReader, paths, globalPaths);

        return new ()
        {
            Id = appReader.AppId,
            IsApp = appReader.NameId != Eav.Constants.DefaultAppGuid &&
                    appReader.NameId != Eav.Constants.PrimaryAppGuid, // #SiteApp v13
            Guid = appReader.NameId,
            Name = appReader.Name,
            Folder = appReader.Folder,
            AppRoot = paths.Path,
            IsHidden = appReader.Configuration.IsHidden,
            ConfigurationId = appReader.Configuration.Id,
            Items = appReader.List.Count,
            Thumbnail = thumbnail,// a.Thumbnail,
            Version = appReader.VersionSafe(),
            IsGlobal = appReader.IsShared(),
            IsInherited = appReader.IsInherited(),
            Lightspeed = lightspeed,
            HasCodeWarnings = codeStats.AppHasWarnings(appReader.AppId),
        };
    }
}