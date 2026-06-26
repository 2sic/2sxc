using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Eav.Models;

namespace ToSic.Sxc.Code.Generate.Sys;

[ModelSpecs(Use = typeof(DataCopilotConfigurationFromEntity))]  // Needed so the model converter knows what to use when converting from entity to model
[PrivateApi]
internal interface IDataCopilotConfiguration: IModelFromEntity
{
    int Id { get; }
    string CodeGenerator { get; }
    bool AutoGenerate { get; }
    string Namespace { get; }
    string TargetFolder { get; }
    string ContentTypes { get; }
    
    /// <summary>
    /// The set determines if it's going to do everything / defaults, or just specific ones.
    /// Values are "" and "custom"
    /// </summary>
    public string ContentTypeSet { get; }
    string Prefix { get; }
    string Suffix { get; }
    string Edition { get; }
}

[ContentTypeSpecs(
    Guid = "b08dcd23-2eb0-4a5e-a3d0-3178d2aae451", // Matches NameId in data
    Description = "Data Copilot Configuration",
    Name = MyContentTypeName
)]
internal record DataCopilotConfiguration: IDataCopilotConfiguration, IRawEntity
{
    internal const string MyContentTypeName = "DataCopilotConfiguration";

    public int Id { get; init; } = 0;
    public Guid Guid { get; init; } = Guid.NewGuid();
    public string Title => CodeGenerator;
    public string CodeGenerator { get; init; } = "";
    public bool AutoGenerate { get; init; } = false;
    public string Namespace { get; init; } = "";
    public string TargetFolder { get; init; } = "";
    public string ContentTypes { get; init; } = "";
    public string ContentTypeSet { get; init; } = "";
    public string Prefix { get; init; } = "";
    public string Suffix { get; init; } = "";
    public string Edition { get; init; } = "";


    DateTime IRawEntity.Created => DateTime.Now;
    DateTime IRawEntity.Modified => DateTime.Now;

    IDictionary<string, object?> IRawEntity.Attributes(RawConvertOptions options) =>
        new Dictionary<string, object?>
        {
            { nameof(CodeGenerator), CodeGenerator },
            { nameof(AutoGenerate), AutoGenerate},
            { nameof(ContentTypes), ContentTypes },
            { nameof(Namespace), Namespace },
            { nameof(TargetFolder), TargetFolder },
            { nameof(Prefix), Prefix },
            { nameof(Suffix), Suffix },
            { nameof(Edition), Edition }
        };
}

[ModelSpecs(ContentType = DataCopilotConfiguration.MyContentTypeName)] // so it knows the real name of the content-type for type checks
internal record DataCopilotConfigurationFromEntity : ModelFromEntityBasic, IDataCopilotConfiguration
{
    [ContentTypeAttributeSpecs(IsTitle = true)]
    public string CodeGenerator => GetThis("");
    
    public bool AutoGenerate => GetThis(false);
    public string Namespace => GetThis("");
    public string TargetFolder => GetThis("");
    public string ContentTypeSet => GetThis("");
    public string ContentTypes => GetThis("");
    public string Prefix => GetThis("");
    public string Suffix => GetThis("");
    public string Edition => GetThis("");


}

internal static class DataCopilotConfigurationExtensions
{
    public static ICollection<string>? GetSelectedContentTypes(this IDataCopilotConfiguration configuration)
    {
        var selected = configuration.ContentTypes
            .CsvToArrayWithoutEmpty()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return selected.Any()
            ? selected
            : null;
    }
}