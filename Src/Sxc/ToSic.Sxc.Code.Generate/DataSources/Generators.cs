using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.DI;

namespace ToSic.Sxc.DataSources;

[PrivateApi]
[VisualQuery(
    ConfigurationType = "",
    NameId = "f512e44b-5b34-4a32-bfe3-d46d46800a7f",
    //HelpLink = "https://go.2sxc.org/ds-sites",
    //Icon = DataSourceIcons.Globe,
    NiceName = "Generators",
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "Generators in this system")]
public class Generators: CustomDataSource
{
    public Generators(Dependencies services, LazySvc<IEnumerable<IFileGenerator>> generators)
        : base(services, logName: "CDS.Generators", connect: [generators])
    {
        ProvideOutRaw(() => GetList(generators.Value), options: () => new()
        {
            AutoId = true,
            TitleField = "Name",
            TypeName = "Generator",
        });
    }

    private IEnumerable<IRawEntity> GetList(IEnumerable<IFileGenerator> fileGenerators)
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();
        //var fileGenerators = _generators.Value;
        var list = fileGenerators
            .Select(g => new RawEntity(new()
            {
                { AttributeNames.NameIdNiceName, g.NameId },
                { nameof(g.Name), g.Name },
                { nameof(g.Version), g.Version },
                { nameof(g.Description), g.Description },
                { nameof(g.DescriptionHtml), g.DescriptionHtml },
                { nameof(g.OutputLanguage), g.OutputLanguage },
                { nameof(g.OutputType), g.OutputType },
            }))
            .ToList();

        return l.Return(list, $"{list.Count}");
    }
}