using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Web.Internal.LightSpeed;

namespace ToSic.Sxc.Backend.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppsBackend: ServiceBase
{
    private readonly Generator<IAppPathsMicroSvc> _appPathsGen;
    private readonly LazySvc<GlobalPaths> _globalPaths;
    private readonly WorkApps _workApps;
    private readonly CodeInfoStats _codeStats;
    private readonly IContextOfSite _context;

    public AppsBackend(WorkApps workApps, IContextOfSite context, CodeInfoStats codeStats, Generator<IAppPathsMicroSvc> appPathsGen, LazySvc<GlobalPaths> globalPaths) : base("Bck.Apps")
    {
        ConnectServices(
            _workApps = workApps,
            _codeStats = codeStats,
            _context = context,
            _appPathsGen = appPathsGen,
            _globalPaths = globalPaths
        );
    }
        
    public List<AppDto> Apps()
    {
        var list = _workApps.GetApps(_context.Site);
        return list.Select(CreateAppDto).ToList();
    }

    public List<AppDto> GetInheritableApps()
    {
        var list = _workApps.GetInheritableApps(_context.Site);
        return list.Select(CreateAppDto).ToList();
    }

    private AppDto CreateAppDto(IAppStateInternal state)
    {
        AppMetadataDto lightspeed = null;
        var lightSpeedDeco = LightSpeedDecorator.GetFromAppStatePiggyBack(state.StateCache, Log);
        if (lightSpeedDeco.Entity != null)
            lightspeed = new AppMetadataDto { Id = lightSpeedDeco.Id, Title = lightSpeedDeco.Title, IsEnabled = lightSpeedDeco.IsEnabled };

        var paths = _appPathsGen.New().Init(_context.Site, state);
        var thumbnail = AppAssetThumbnail.GetUrl(state, paths, _globalPaths);

        return new AppDto
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
            HasCodeWarnings = _codeStats.AppHasWarnings(state.AppId),
        };
    }
}