using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the App folders,
/// until it's converted to an IEntity in the <see cref="AppAssets"/> DataSource.
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
[ContentTypeSpecs(
    Guid = "96cda931-b677-4589-9eb2-df5a38cefff0",
    Description = "Folder in an App"
)]
public class AppFolderDataRaw: AppFileDataRawBase
{
    public const string TypeName = "Folder";

    public static DataFactoryOptions Options = new(typeName: TypeName, titleField: nameof(Path));

    /// <summary>
    /// The folder name - or blank when it's the root.
    /// </summary>
    [ContentTypeAttributeSpecs(Description = "The folder name or blank when it's the root.")]
    public override string Name { get; set; }

    ///// <summary>
    ///// The folder name.
    ///// </summary>
    //[ContentTypeAttributeSpecs(IsTitle = true, Description = "The folder name or 'root' when it's the root")]
    ////public string Title { get => field.NullIfNoValue() ?? "root"; set => field = value; }
    //public string Title => Name.NullIfNoValue() ?? "root";


    [PrivateApi]
    public override IDictionary<string, object> Attributes(RawConvertOptions options)
        => new Dictionary<string, object>(base.Attributes(options))
        {
            { "Folders", new RawRelationship(key: $"FolderIn:{Path}") },
            { "Files", new RawRelationship(key: $"FileIn:{Path}") },
            //{ nameof(Title), Title },
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