using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.Data.Sys.ContentTypes;

namespace ToSic.Sxc.Cms.Assets.Internal;

/// <summary>
/// Internal class to hold all the information about the App folders,
/// until it's converted to an IEntity in the <see cref="AppAssets"/> DataSource.
///
/// Important: this is an internal object.
/// We're just including in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeSpecs(
    Guid = "96cda931-b677-4589-9eb2-df5a38cefff0",
    Description = "Folder in an App",
    Name = TypeName
)]
public record FolderModelRaw: FileFolderBase, IFolderModelSync
{
    internal const string TypeName = "Folder";

    internal static DataFactoryOptions Options = new()
    {
        TypeName = TypeName,
        TitleField = nameof(Path)
    };

    /// <inheritdoc cref="IFolderModelSync.Name"/>
    [ContentTypeAttributeSpecs(Description = "The folder name or blank when it's the root.")]
    public override string? Name { get; init; }

    [ContentTypeAttributeSpecs(Type = ValueTypes.Entity, Description = "Folders in this folder.")]
    public RawRelationship Folders => new(key: $"FolderIn:{Path}");

    [ContentTypeAttributeSpecs(Type = ValueTypes.Entity, Description = "Files in this folder.")]
    public RawRelationship Files => new(key: $"FileIn:{Path}");

    [PrivateApi]
    public override IDictionary<string, object?> Attributes(RawConvertOptions options)
        => new Dictionary<string, object?>(base.Attributes(options))
        {
            { nameof(Folders), Folders },
            { nameof(Files), Files },
        };

    [PrivateApi]
    public override IEnumerable<object> RelationshipKeys(RawConvertOptions options)
        => new List<object>
        {
            // For Relationships looking for this folder
            $"Folder:{Path}",
            // For Relationships looking for folders having a specific parent
            $"FolderIn:{ParentFolderInternal}",
        };

}