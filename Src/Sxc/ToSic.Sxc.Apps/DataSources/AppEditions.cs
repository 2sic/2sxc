using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.DataSources;

[PrivateApi]
[VisualQuery(
    ConfigurationType = "",
    NameId = "6cce259b-bf0c-4752-b451-a3eb04825350",
    //HelpLink = "https://go.2sxc.org/ds-sites",
    //Icon = DataSourceIcons.Globe,
    NiceName = "Editions",
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "Editions in this application")]

public class AppEditions : CustomDataSource
{
    public AppEditions(Dependencies services, LazySvc<IAppJsonConfigurationService> appJsonService)
        : base(services, logName: "App.EditDS", connect: [appJsonService])
    {
        ProvideOutRaw(() => GetList(appJsonService.Value), options: () => new()
        {
            AutoId = false,
            TitleField = "Name",
            TypeName = "Edition",
        });
    }

    private IEnumerable<IRawEntity> GetList(IAppJsonConfigurationService appJsonService)
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();
        var appJson = appJsonService.GetAppJson(AppId);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if (appJson?.Editions?.Count > 0)
        {
            l.A($"has editions in app.json: {appJson.Editions.Count}");
            var list = appJson.Editions
                .Select(s => new RawEntity(new()
                {
                    { "Name", s.Key },
                    { "Description", s.Value.Description },
                    { "IsDefault", s.Value.IsDefault },
                }))
                .ToList();
            return l.Return(list, $"{list.Count}");
        }

        // default data
        var rootEdition = new RawEntity(new()
        {
            { "Name", "" },
            { "Description", "Root edition" },
            { "IsDefault", true },
        });
        return l.Return([rootEdition], $"editions are not specified, so using default edition data");
    }
}