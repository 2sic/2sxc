using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.App;

namespace ToSic.Eav.WebApi.Sys.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "Apps",
    NameId = "53b3fe9b-d689-4b1f-bed1-503cbc898ffc",
    NameIds = ["System.Apps"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Apps of the current site"
)]
public class Apps : CustomDataSource
{
    private readonly LazySvc<AppsBackend> _appsBackend;

    public Apps(
        Dependencies services,
        LazySvc<AppsBackend> appsBackend)
        : base(services, logName: "Sxc.Apps", connect: [appsBackend])
    {
        _appsBackend = appsBackend;

        ProvideOutRaw(GetApps, options: () => new()
        {
            TitleField = nameof(AppDto.Name),
            TypeName = "App",
        });
    }

    private IEnumerable<IRawEntity> GetApps()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();

        var entities = _appsBackend.Value.Apps()
            .Select(app => new RawEntity(new()
            {
            { nameof(AppDto.IsApp), app.IsApp },
            { nameof(AppDto.Guid), app.Guid },
            { nameof(AppDto.Name), app.Name },
            { nameof(AppDto.Folder), app.Folder },
            { nameof(AppDto.AppRoot), app.AppRoot },
            { nameof(AppDto.IsHidden), app.IsHidden },
            { nameof(AppDto.ConfigurationId), app.ConfigurationId },
            { nameof(AppDto.Items), app.Items },
            { nameof(AppDto.Thumbnail), app.Thumbnail },
            { nameof(AppDto.Version), app.Version },
            { nameof(AppDto.IsGlobal), app.IsGlobal },
            { nameof(AppDto.IsInherited), app.IsInherited },
            { nameof(AppDto.Lightspeed), app.Lightspeed },
            { nameof(AppDto.HasCodeWarnings), app.HasCodeWarnings },
            })
            {
                Id = app.Id
            });

        return l.Return(entities, "ok");
    }
}