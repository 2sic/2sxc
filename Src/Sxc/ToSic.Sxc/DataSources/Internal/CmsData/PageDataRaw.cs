using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the page,
/// until it's converted to an IEntity in the <see cref="DataSources.Pages"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn TabInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Tabs.TabInfo.html)
/// * [Oqtane Page](https://docs.oqtane.org/api/Oqtane.Models.Page.html)
/// 
/// Important: this is an internal object.
/// We're just including in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[ContentTypeSpecs(
    Guid = "c648a91d-b650-42bf-ad6a-9582015c165e",
    Description = "Page in the site",
    Name = TypeName
)]
public class PageDataRaw: IRawEntity, IPageModel, IHasRelationshipKeys
{
    private const string TypeName = "Page";

    public static DataFactoryOptions Option = new()
    {
        TypeName = TypeName,
        TitleField = nameof(Title)
    };

    /// <inheritdoc cref="IPageModel.Id"/>
    public int Id { get; init; }

    /// <inheritdoc />
    public int ParentId { get; init; }

    /// <inheritdoc cref="IPageModel.Guid"/>
    public Guid Guid { get; init; }

    /// <inheritdoc cref="IPageModel.Title"/>
    public string Title { get; init; }

    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public bool IsClickable { get; init; }


    /// <inheritdoc />
    public int Order { get; init; }

    /// <inheritdoc />
    public bool IsNavigation { get; init; }

    /// <inheritdoc />
    public bool HasChildren { get; init; }

    /// <inheritdoc />
    public int Level { get; init; }

    /// <inheritdoc />
    public string LinkTarget { get; init; }


    /// <inheritdoc />
    public string Path { get; init; }

    /// <inheritdoc />
    public string Url { get; init; }

    /// <inheritdoc cref="IPageModel.Created" />
    public DateTime Created { get; init; }

    /// <inheritdoc cref="IPageModel.Modified" />
    public DateTime Modified { get; init; }

    /// <inheritdoc />
    public bool IsDeleted { get; init; }

    [ContentTypeAttributeSpecs(Type = ValueTypes.Entity, Description = "Reference to the child pages.")]
    public RawRelationship Children => new(key: $"ParentId:{Id}");

    /// <summary>
    /// Data without ID, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>
    {
        // v14+
        { nameof(Title), Title },
        { nameof(Name), Name },
        { nameof(ParentId), ParentId },
        { nameof(IsNavigation), IsNavigation },
        { nameof(Path), Path },
        { nameof(Url), Url },
        // New in v15.01
        { nameof(IsClickable), IsClickable },
        { nameof(Order), Order },
        { nameof(IsDeleted), IsDeleted },
        { nameof(Level), Level },
        { nameof(HasChildren), HasChildren },
        // New in v15.02
        { nameof(LinkTarget), LinkTarget },

        { nameof(Children), Children }
    };

    public IEnumerable<object> RelationshipKeys(RawConvertOptions options)
        => new List<object>
        {
            // For relationships looking for files in this folder
            $"ParentId:{ParentId}"
        };
}