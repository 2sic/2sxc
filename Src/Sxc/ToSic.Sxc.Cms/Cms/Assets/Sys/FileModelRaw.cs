using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Sys.ContentTypes;

namespace ToSic.Sxc.Cms.Assets.Sys;

/// <summary>
/// Internal class to hold all the information about the App files,
/// until it's converted to an IEntity in the <see cref="AppAssets"/> DataSource.
///
/// Important: this is an internal object.
/// We're just including it in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// * Make sure the property names never change, as they are critical for the created Entity.
/// * Was InternalApi till v17 - hide till we know how to handle to-typed-conversions
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeSpecs(
    Guid = "3cf0822f-d276-469a-bbd1-cc84fd6ff748",
    Description = "File in an App",
    Name = TypeName
)]
public record FileModelRaw: FileFolderBase, IFileModelSync
{
    internal const string TypeName = "File";

    /// <inheritdoc cref="IFileModelSync.Name"/>
    [ContentTypeField(Description = "The file name without extension, like my-image")]
    public override string? Name { get; init; }

    /// <inheritdoc cref="IFileModelSync.Extension"/>
    public string? Extension { get; init; }

    /// <inheritdoc cref="IFileModelSync.Size"/>
    public int Size { get; init; }

    [PrivateApi]
    public override IDictionary<string, object?> Values => field ??= new Dictionary<string, object?>(base.Values)
    {
        { nameof(Extension), Extension },
        { nameof(Size), Size },
    };

    [PrivateApi]
    public override IEnumerable<object> RelationshipKeys => field ??= new List<object>
    {
        // For relationships looking for files in this folder
        $"FileIn:{ParentFolderInternal}"
    };
}