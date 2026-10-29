using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources;

[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentType(Guid = "7f3f0fcf-9186-4c16-9e10-88f785ec5062", Name = TypeName)]
public class AdamItemDataRaw: IRawEntity
{
    public const string TypeName = "AdamItem";

    public int Id { get; set; }
    public Guid Guid { get; set; }

    /// <summary>
    /// The file name
    /// </summary>
    [ContentTypeTitle]
    public string? Name { get; set; }

    /// <summary>
    /// This contains the code like "file:2742"
    /// </summary>
    public string? ReferenceId { get; set; }

    /// <summary>
    /// Normal url to access the resource
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The Adam type, such as "folder", "image" etc.
    /// </summary>
    public string? Type { get; set; }

    public bool IsFolder { get; set; }

    public int Size { get; set; }
    public string? Path { get; set; }
        

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    IDictionary<string, object?> IRawEntity.Values => field ??= new Dictionary<string, object?>
    {
        { nameof(Name), Name },
        { nameof(ReferenceId), ReferenceId },
        { nameof(Url), Url },
        { nameof(Type), Type },
        { nameof(IsFolder), IsFolder },
        { nameof(Size), Size },
        { nameof(Path), Path }
    };

}