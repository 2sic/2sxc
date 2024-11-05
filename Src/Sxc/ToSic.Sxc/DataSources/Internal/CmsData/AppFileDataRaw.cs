using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the App files,
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
    Guid = "3cf0822f-d276-469a-bbd1-cc84fd6ff748",
    Description = "File in an App"
)]
public class AppFileDataRaw: AppFileDataRawBase
{
    public const string TypeName = "File";

    public static DataFactoryOptions Options = new(typeName: TypeName, titleField: nameof(Name));

    /// <summary>
    /// The file name extension
    /// </summary>
    public string Extension { get; set; }

    public long Size { get; set; }

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public override IDictionary<string, object> Attributes(RawConvertOptions options)
        => new Dictionary<string, object>(base.Attributes(options))
        {
            { nameof(Extension), Extension },
            { nameof(Size), Size },
        };

    [PrivateApi]
    public override IEnumerable<object> RelationshipKeys(RawConvertOptions options)
        => new List<object> { $"FileIn:{ParentFolderInternal}" };

}