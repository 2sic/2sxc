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

    private AppDto CreateAppDto(IAppStateInternal state)
    {
        AppMetadataDto lightspeed = null;
        var lightSpeedDeco = LightSpeedDecorator.GetFromAppStatePiggyBack(state.StateCache, Log);
        if (lightSpeedDeco.Entity != null)
            lightspeed = new () { Id = lightSpeedDeco.Id, Title = lightSpeedDeco.Title, IsEnabled = lightSpeedDeco.IsEnabled };

        var paths = appPathsGen.New().Init(context.Site, state);
        var thumbnail = AppAssetThumbnail.GetUrl(state, paths, globalPaths);

        return new ()
        {
            Id = state.AppId,
            IsApp = state.NameId != Eav.Constants.DefaultAppGuid &&
                    state.NameId != Eav.Constants.PrimaryAppGuid, // #SiteApp v13
            Guid = state.NameId,
            Name = state.Name,
            Folder = state.Folder,
            AppRoot = paths.Path,
            IsHidden = state.Configuration.IsHidden,
            ConfigurationId = state.Configuration.Id,
            Items = state.List.Count,
            Thumbnail = thumbnail,// a.Thumbnail,
            Version = state.VersionSafe(),
            IsGlobal = state.IsShared(),
            IsInherited = state.IsInherited(),
            Lightspeed = lightspeed,
            HasCodeWarnings = codeStats.AppHasWarnings(state.AppId),
        };
    }
}