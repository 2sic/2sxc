using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.Dto;


namespace ToSic.Sxc.Backend.DataSources
{
    [PublicApi]
    [VisualQuery(
        ConfigurationType = "",
        NameId = "6cce259b-bf0c-4752-b451-a3eb04825350",
        //HelpLink = "https://go.2sxc.org/ds-sites",
        //Icon = DataSourceIcons.Globe,
        NiceName = "Editions",
        Type = DataSourceType.System,
        UiHint = "Editions in this application")]

    public class Editions : CustomDataSource
    {
        public Editions(Dependencies services, LazySvc<IAppJsonConfigurationService> appJsonService) : base(services, logName: "CDS.Editions", connect: [appJsonService])
        {
            _appJsonService = appJsonService;
            ProvideOutRaw(GetList, options: () => new()
            {
                AutoId = false,
                TitleField = "Name",
                TypeName = "Edition",
            });
        }

        private readonly LazySvc<IAppJsonConfigurationService> _appJsonService;

        private IEnumerable<IRawEntity> GetList()
        {
            var l = Log.Fn<IEnumerable<IRawEntity>>();
            var appJson = _appJsonService.Value.GetAppJson(AppId);

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            if (appJson?.Editions?.Count > 0)
            {
                l.A($"has editions in app.json: {appJson.Editions.Count}");
                var list = appJson.Editions
                    .Select(s => new RawEntity(new()
                    {
                        { AttributeNames.NameIdNiceName, s.Key },
                        { "Name", s.Value.Description },
                        { "Description", s.Value.Description },
                        { "IsDefault", s.Value.IsDefault },
                    }))
                    .ToList();
                return l.Return(list, $"{list.Count}");
            }

            // default data
            var rootEdition = new RawEntity(new()
            {
                { AttributeNames.NameIdNiceName, "" },
                { "Name", "" },
                { "Description", "Root edition" },
                { "IsDefault", true },
            });
            return l.Return([rootEdition], $"editions are not specified, so using default edition data");
        }
    }
}
