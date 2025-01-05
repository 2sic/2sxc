using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract record AppFileDataRawBase: IRawEntity, IHasRelationshipKeys
{
    /// <inheritdoc />
    [ContentTypeAttributeSpecs(Description = "DO NOT USE. This is a temporary, random ID calculated at runtime and will return different values all the time.")]
    public int Id => 0;

    /// <inheritdoc />
    [ContentTypeAttributeSpecs(Description = "DO NOT USE. This is a temporary, random ID calculated at runtime and will return different values all the time.")]
    public Guid Guid => default;

    public abstract string Name { get; set; }

    /// <summary>
    /// The full name with extension.
    /// If it's a folder or there is no extension, then it's identical to the <see cref="Name"/>
    /// </summary>
    [ContentTypeAttributeSpecs(Description = "The full name with the path beginning at the root. Note that the root can differ depending on the files you ask for.")]
    public string FullName { get; set; }

    /// <summary>
    /// This is just for internal lookup
    /// </summary>
    [ContentTypeAttributeIgnore]
    public string ParentFolderInternal { get; set; }

    /// <summary>
    /// Starting in the App-Root
    /// </summary>
    [ContentTypeAttributeSpecs(IsTitle = true, Description = "Full path. It starts at the root of the app or whatever other system you're asking for. Always end with slash, so root is `/` and it's easy to distinguish folders and files.")]
    public string Path { get; set; }

    /// <inheritdoc />
    [ContentTypeAttributeSpecs(Description = "When the file/folder was created.")]
    public DateTime Created { get; set; }

    /// <inheritdoc />
    [ContentTypeAttributeSpecs(Description = "When the file/folder was modified.")]
    public DateTime Modified { get; set; }

    /// <summary>
    /// The full url starting at the root of the site. Absolute but without protocol/domain.
    /// </summary>
    [ContentTypeAttributeSpecs(Description = "The full url starting at the root of the site. Absolute but without protocol/domain.")]
    public string Url { get; set; }

    [ContentTypeAttributeSpecs(Type = ValueTypes.Entity, Description = "Reference to the parent folder.")]
    public RawRelationship Folder => new(key: $"Folder:{ParentFolderInternal}");

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public virtual IDictionary<string, object> Attributes(RawConvertOptions options)
        => new Dictionary<string, object>
        {
            { nameof(Name), Name },
            { nameof(FullName), FullName },
            { nameof(Path), Path },
            { nameof(Url), Url },
            { nameof(Folder), Folder },

            // For debugging
            //{ nameof(ParentFolderInternal), ParentFolderInternal },
        };

    [PrivateApi]
    public abstract IEnumerable<object> RelationshipKeys(RawConvertOptions options);
}