using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.DataSources;

[PrivateApi]
[VisualQuery(
    NiceName = "App Editions",
    NameId = "6cce259b-bf0c-4752-b451-a3eb04825350",
    NameIds = ["System.AppEditions"], // for use in the front end
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "App Editions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppEditions(CustomDataSource.Dependencies services, IAppJsonConfigurationService appJsonService)
    : CustomDataSource(services, logName: "App.EditDS", connect: [appJsonService])
{

    protected override IEnumerable<IRawData> GetDefault()
    {
        var l = Log.Fn<IEnumerable<AppEditionRaw>>();
        var appJson = appJsonService.GetAppJson(AppId);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if (!(appJson?.Editions?.Count > 0))
            return l.Return(
                [new("", "Root edition", true)],
                "editions are not specified, so using default edition data");

        l.A($"has editions in app.json: {appJson.Editions.Count}");
        var list = appJson.Editions
            .Select(s => new AppEditionRaw(s.Key, s.Value.Description, s.Value.IsDefault))
            .ToList();
        return l.Return(list, $"{list.Count}");

    }


    [ContentType(
        Name = "Edition",
        Guid = "5b93ae50-bfc2-403b-aa71-7f7e4e109657",
        Scope = "System")]
    private sealed record AppEditionRaw(
        [property: ContentTypeTitle] string Name,
        string? Description,
        bool IsDefault
    ) : IRawEntityAutoConvert;
}
