using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Eav.Models;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// The interface is the main thing we should use.
/// It also tells ToModel{IDataCopilotConfiguration}() what to use when converting from entity to model.
/// </summary>
[ModelSpecs(Use = typeof(DataCopilotConfigurationFromEntity))]  // Needed so the model converter knows what to use when converting from entity to model
[PrivateApi]
internal interface IDataCopilotConfiguration: IModelFromEntity
{
    /// <summary>
    /// ID as stored in the entity.
    /// Not really relevant, but we want log it in certain cases, so we include it in the config.
    /// </summary>
    int Id { get; }
    
    /// <summary>
    /// .net class Name (without namespace) of the code generator to use.
    /// </summary>
    /// <remarks>
    /// Will be used to find the instance of the code generator, based on all types which implement the interface and have this name.
    /// </remarks>
    string CodeGenerator { get; }
    
    /// <summary>
    /// Determines if the configuration will be used to auto-generate code files on changes in the type/fields.
    /// </summary>
    bool AutoGenerate { get; }
    
    /// <summary>
    /// Output namespace to use in the generated files, typically `AppCode.Data`
    /// </summary>
    string Namespace { get; }

    /// <summary>
    /// Target folder where the generated files should be placed.Usually `AppCode/Data`
    /// </summary>
    string TargetFolder { get; }
    
    /// <summary>
    /// The set determines if it's going to do everything / defaults, or just specific ones.
    /// Values are `""` and `"custom"`
    /// </summary>
    public string ContentTypeSet { get; }
    
    /// <summary>
    /// Specific names of content types to process - if the ContentTypeSet is not `""`
    /// </summary>
    string ContentTypes { get; }
    
    /// <summary>
    /// Prefix to add to the generated class names. Default is `""`.
    /// </summary>
    string Prefix { get; }
    
    /// <summary>
    /// Suffix to add to the generated class names. Default is `""`, but it's often `Model` for model only.
    /// </summary>
    string Suffix { get; }
    
    /// <summary>
    /// Edition to generate into, affects the target folder used.
    /// </summary>
    string Edition { get; }
}

/// <summary>
/// This class has 2 purposes:
///
/// 1. Contains the structure (for auto-generated ContentType definitions)
/// 2. Is the "raw" configuration which can be used to generate entities of this type - for example in testing.
/// </summary>
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

    #region IRawEntity implementation to enable conversion to IEntity
    
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

    #endregion

    IConvertToRawEntity? IGetRawConverter.GetConverter() => null;
}

/// <summary>
/// This is the IEntity wrapper implementing the interface.
/// It will be used automatically when calling ToModel{IDataCopilotConfiguration}() on an entity of this type.
/// </summary>
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

/// <summary>
/// Helpers for the DataCopilotConfiguration
/// </summary>
internal static class DataCopilotConfigurationExtensions
{
    /// <summary>
    /// Convert names in the <see cref="IDataCopilotConfiguration.ContentTypes"/> property to a collection of strings - or `null` if empty.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
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