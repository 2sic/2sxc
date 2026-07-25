using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Raw.Sys;
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

        ProvideOutRaw(GetApps, options: () => new()
        {
            TitleField = nameof(AppDto.Name),
            TypeName = "App",
        });
    }

    private IEnumerable<IRawEntity> GetApps()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();

        if (!_user.Value.IsSystemAdmin)
            throw HttpException.PermissionDenied("Listing inheritable apps requires SuperUser permissions.");

        var apps = _workApps.GetInheritableApps(_context.Site)
            .Select(ToRawEntity);

        return l.Return(apps, "ok");
    }

    private IRawEntity ToRawEntity(IAppReader appReader)
    {
        var specs = appReader.Specs;
        var paths = _appPathsGen.New().Get(appReader, _context.Site);

        var isApp = specs.NameId != KnownAppsConstants.DefaultAppGuid &&
                    specs.NameId != KnownAppsConstants.PrimaryAppGuid;

        return new RawEntity
        {
            Id = appReader.AppId,
            Values = new Dictionary<string, object?>
            {
                { nameof(AppDto.IsApp), isApp },
                { nameof(AppDto.Guid), specs.NameId },
                { nameof(AppDto.Name), specs.Name },
                { nameof(AppDto.Folder), specs.Folder },
                { nameof(AppDto.AppRoot), paths.Path },
                { nameof(AppDto.IsHidden), specs.Configuration.IsHidden },
                { nameof(AppDto.ConfigurationId), specs.Configuration.Id },
                { nameof(AppDto.Items), appReader.List.Count },
                { nameof(AppDto.Thumbnail), AppAssetThumbnail.GetUrl(appReader, paths, _globalPaths) },
                { nameof(AppDto.Version), specs.VersionSafe() },
                { nameof(AppDto.IsGlobal), appReader.IsShared() },
                { nameof(AppDto.IsInherited), appReader.IsInherited() },
                { nameof(AppDto.Lightspeed), LightSpeed(appReader) },
                { nameof(AppDto.HasCodeWarnings), _codeStats.AppHasWarnings(appReader.AppId) },
            },
        };
    }

    private static AppMetadataDto? LightSpeed(IAppReader appReader)
    {
        var lightSpeed = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader);
        return (lightSpeed as ICanBeEntity)?.Entity == null
            ? null
            : new() { Id = lightSpeed.Id, Title = lightSpeed.Title, IsEnabled = lightSpeed.IsEnabled };
    }
}
