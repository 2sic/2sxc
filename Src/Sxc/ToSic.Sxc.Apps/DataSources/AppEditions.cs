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
public class AppEditions : CustomDataSource
{
    public AppEditions(Dependencies services, IAppJsonConfigurationService appJsonService)
        : base(services, logName: "App.EditDS", connect: [appJsonService])
        => ProvideOutRaw(() => GetList(appJsonService));

    private IEnumerable<AppEditionRaw> GetList(IAppJsonConfigurationService appJsonService)
    {
        var l = Log.Fn<IEnumerable<AppEditionRaw>>();
        var appJson = appJsonService.GetAppJson(AppId);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if (appJson?.Editions?.Count > 0)
        {
            l.A($"has editions in app.json: {appJson.Editions.Count}");
            var list = appJson.Editions
                .Select(s => new AppEditionRaw(s.Key, s.Value.Description, s.Value.IsDefault))
                .ToList();
            return l.Return(list, $"{list.Count}");
        }

        return l.Return(
            [new AppEditionRaw("", "Root edition", true)],
            "editions are not specified, so using default edition data");
    }
}

[ContentType(Name = "Edition", Guid = "5b93ae50-bfc2-403b-aa71-7f7e4e109657", Scope = "System")]
internal sealed record AppEditionRaw(
    [property: ContentTypeTitle] string Name,
    string? Description,
    bool IsDefault
) : IRawEntityAutoConvert;
