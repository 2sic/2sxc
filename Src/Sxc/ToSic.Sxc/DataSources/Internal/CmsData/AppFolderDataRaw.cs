using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the App folders,
/// until it's converted to an IEntity in the <see cref="AppFiles"/> DataSource.
///
/// Important: this is an internal object.
/// We're just including in in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFolderDataRaw: AppFileDataRawBase
{
    public const string TypeName = "Folder";

    public static DataFactoryOptions Options = new(typeName: TypeName, titleField: nameof(Name));

    [PrivateApi]
    public override IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>(base.Attributes(options))
    {
        { "Folders", new RawRelationship(key: $"FolderIn:{FullName}") },
        { "Files", new RawRelationship(key: $"FileIn:{FullName}") },
    };

    [PrivateApi]
    public override IEnumerable<object> RelationshipKeys(RawConvertOptions options) => new List<object>
    {
        // For Relationships looking for this folder
        $"Folder:{FullName}",
        // For Relationships looking for folders having a specific parent
        $"FolderIn:{ParentFolderInternal}",
    };

}