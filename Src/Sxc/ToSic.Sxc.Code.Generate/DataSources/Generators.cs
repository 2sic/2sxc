using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.DI;

namespace ToSic.Sxc.DataSources
{
    [PublicApi]
    [VisualQuery(
        ConfigurationType = "",
        NameId = "f512e44b-5b34-4a32-bfe3-d46d46800a7f",
        //HelpLink = "https://go.2sxc.org/ds-sites",
        //Icon = DataSourceIcons.Globe,
        NiceName = "Generators",
        Type = DataSourceType.System,
        UiHint = "Generators in this system")]

    public class Generators: CustomDataSource
    {
        public Generators(Dependencies services, LazySvc<IEnumerable<IFileGenerator>> generators) : base(services, logName: "CDS.Generators", connect: [generators])
        {
            _generators = generators;
            ProvideOutRaw(GetList, options: () => new()
            {
                AutoId = true,
                TitleField = "Name",
                TypeName = "Generator",
            });
        }

        private readonly LazySvc<IEnumerable<IFileGenerator>> _generators;

        private IEnumerable<IRawEntity> GetList()
        {
            var l = Log.Fn<IEnumerable<IRawEntity>>();
            var fileGenerators = _generators.Value;
            var list = fileGenerators
                .Select(g => new RawEntity(new()
                {
                    { AttributeNames.NameIdNiceName, g.NameId },
                    { "Name", g.Name },
                    { "Version", g.Version },
                    { "Description", g.Description },
                    { "DescriptionHtml", g.DescriptionHtml },
                    { "OutputLanguage", g.OutputLanguage },
                    { "OutputType", g.OutputType },
                }))
                .ToList();

            return l.Return(list, $"{list.Count}");
        }
    }

}
