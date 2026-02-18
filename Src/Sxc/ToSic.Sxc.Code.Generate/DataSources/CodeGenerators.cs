using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.DI;

namespace ToSic.Sxc.DataSources;

[PrivateApi]
[VisualQuery(
    NiceName = "Code Generators",
    NameId = "f512e44b-5b34-4a32-bfe3-d46d46800a7f",
    NameIds = ["System.CodeGenerators"], // Internal name for the system, used in some entity-pickers. Can change at any time.
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.System,
    UiHint = "Generators in this system")]
// ReSharper disable once UnusedMember.Global
public class CodeGenerators: CustomDataSource
{
    public CodeGenerators(Dependencies services, LazySvc<IEnumerable<IFileGenerator>> generators)
        : base(services, logName: "CDS.Generators", connect: [generators])
    {
        ProvideOutRaw(
            () => Generators(generators.Value),
            options: () => new()
            {
                AutoId = true,
                TitleField = "Name",
                TypeName = "Generator",
            });

        ProvideOutRaw(
            () => OutputTypes(generators.Value),
            name: "OutputType",
            options: () => new()
            {
                AutoId = true,
                TitleField = "Name",
                TypeName = "OutputType",
            });

    }

    private IEnumerable<IRawEntity> Generators(IEnumerable<IFileGenerator> fileGenerators)
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();
        var list = fileGenerators
            .Select(g => new RawEntity(new()
            {
                //{ AttributeNames.NameIdNiceName, g.NameId },
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

    private IEnumerable<IRawEntity> OutputTypes(IEnumerable<IFileGenerator> fileGenerators)
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();
        var list = fileGenerators
            .GroupBy(g => g.OutputType)
            .Select(g => new RawEntity(new()
            {
                { "Name", g.Key },
            }))
            .ToList();

        return l.Return(list, $"{list.Count}");
    }

}