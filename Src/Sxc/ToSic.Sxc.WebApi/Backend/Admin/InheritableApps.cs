using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Code.InfoSystem;
using ToSic.Sys.Users;

namespace ToSic.Eav.WebApi.Sys.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "Inheritable Apps",
    NameId = "64b81dd8-23b6-4a37-bc43-78661e3a3e2c",
    NameIds = ["System.InheritableApps"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "Apps from other sites which the current site can inherit"
)]
public class InheritableApps : CustomDataSource
{
    private readonly WorkApps _workApps;
    private readonly IContextOfSite _context;
    private readonly CodeInfoStats _codeStats;
    private readonly Generator<IAppPathsMicroSvc> _appPathsGen;
    private readonly LazySvc<GlobalPaths> _globalPaths;
    private readonly LazySvc<IUser> _user;

    public InheritableApps(
        Dependencies services,
        WorkApps workApps,
        IContextOfSite context,
        CodeInfoStats codeStats,
        Generator<IAppPathsMicroSvc> appPathsGen,
        LazySvc<GlobalPaths> globalPaths,
        LazySvc<IUser> user)
        : base(services, logName: "Sxc.InhApps", connect: [workApps, context, codeStats, appPathsGen, globalPaths, user])
    {
        _workApps = workApps;
        _context = context;
        _codeStats = codeStats;
        _appPathsGen = appPathsGen;
        _globalPaths = globalPaths;
        _user = user;

        ProvideOutRaw(GetApps);
        //    , options: () => new()
        //{
        //    TitleField = nameof(AppRaw.Name),
        //    TypeName = "App",
        //});
    }

    private IEnumerable<AppRaw> GetApps()
    {
        var l = Log.Fn<IEnumerable<AppRaw>>();

        if (!_user.Value.IsSystemAdmin)
            throw HttpException.PermissionDenied("Listing inheritable apps requires SuperUser permissions.");

        var apps = _workApps.GetInheritableApps(_context.Site)
            .Select(ToModel);

        return l.Return(apps, "ok");
    }

    private AppRaw ToModel(IAppReader appReader)
    {
        var specs = appReader.Specs;
        var paths = _appPathsGen.New().Get(appReader, _context.Site);

        var app = new AppRaw
        {
            Id = appReader.AppId,
            IsApp = specs.NameId != KnownAppsConstants.DefaultAppGuid && specs.NameId != KnownAppsConstants.PrimaryAppGuid,
            Guid = specs.NameId,
            Name = specs.Name,
            Folder = specs.Folder,
            AppRoot = paths.Path,
            IsHidden = specs.Configuration.IsHidden,
            ConfigurationId = specs.Configuration.Id,
            Items = appReader.List.Count,
            Thumbnail = AppAssetThumbnail.GetUrl(appReader, paths, _globalPaths),
            Version = specs.VersionSafe(),
            IsGlobal = appReader.IsShared(),
            IsInherited = appReader.IsInherited(),
            Lightspeed = LightSpeed(appReader),
            HasCodeWarnings = _codeStats.AppHasWarnings(appReader.AppId),
        };
        return app;
        //return new AppModel(app)
        //{
        //    Id = appReader.AppId,
        //};
    }

    private static AppMetadataDto? LightSpeed(IAppReader appReader)
    {
        var lightSpeed = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader);
        return (lightSpeed as ICanBeEntity)?.Entity == null
            ? null
            : new()
            {
                Id = lightSpeed.Id,
                Title = lightSpeed.Title,
                IsEnabled = lightSpeed.IsEnabled
            };
    }
}
