using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.ImportExport;

namespace ToSic.Eav.WebApi.Sys.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "App Statistics",
    NameId = "2bb27fae-fc52-412c-91f0-3276a4567dec",
    NameIds = ["System.AppStatistics"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Export statistics for the current app"
)]
public class AppStatistics : CustomDataSource
{
    private readonly LazySvc<ExportApp> _exportAppLazy;

    [Configuration(Field = "ZoneId")]
    public int OfZoneId => Configuration.GetThis(ZoneId);

    public AppStatistics(Dependencies services, LazySvc<ExportApp> exportAppLazy)
        : base(services, logName: "Sxc.AppStats", connect: [exportAppLazy])
    {
        _exportAppLazy = exportAppLazy;

        ProvideOutRaw(GetStatistics, options: () => new()
        {
            AutoId = true,
            TitleField = nameof(AppExportInfoDto.Name),
            TypeName = "AppStatistics",
        });
    }

    private IEnumerable<IRawEntity> GetStatistics()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();

        var app = _exportAppLazy.Value.GetAppInfo(OfZoneId, AppId);
        var list = new[]
        {
            new RawEntity(new()
            {
                { nameof(AppExportInfoDto.Name), app.Name },
                { nameof(AppExportInfoDto.Guid), app.Guid },
                { nameof(AppExportInfoDto.Version), app.Version },
                { nameof(AppExportInfoDto.EntitiesCount), app.EntitiesCount },
                { nameof(AppExportInfoDto.LanguagesCount), app.LanguagesCount },
                { nameof(AppExportInfoDto.TemplatesCount), app.TemplatesCount },
                { nameof(AppExportInfoDto.HasRazorTemplates), app.HasRazorTemplates },
                { nameof(AppExportInfoDto.HasTokenTemplates), app.HasTokenTemplates },
                { nameof(AppExportInfoDto.FilesCount), app.FilesCount },
                { nameof(AppExportInfoDto.TransferableFilesCount), app.TransferableFilesCount },
            })
        };

        return l.Return(list, $"{list.Length}");
    }
}