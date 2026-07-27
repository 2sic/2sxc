using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw.Sys;

namespace ToSic.Sxc.Cms.Pages.Sys;

/// <summary>
/// Internal class to hold all the information about the page,
/// until it's converted to an IEntity in the <see cref="DataSources.Pages"/> DataSource.
///
/// * [Dnn TabInfo](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Tabs.TabInfo.html)
/// * [Oqtane Page](https://docs.oqtane.org/api/Oqtane.Models.Page.html)
/// </summary>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeAssign(Type = typeof(IPageModel))]
public record PageModelRaw: IRawEntity, IPageModel, IRelationshipKeys
{
    #region IRawEntity

    internal static DataFactoryOptions Option = new()
    {
        TitleField = nameof(Title),
        Type = typeof(PageModelRaw)
    };

    IDictionary<string, object?> IRawEntity.Values => field ??= new Dictionary<string, object?>
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

        { "Children", ChildrenRaw }
    };

    private const string ParentPrefix = "ParentId:";

    private RawRelationship ChildrenRaw => new() { Keys = [$"{ParentPrefix}{Id}"] };


    IEnumerable<object> IRelationshipKeys.RelationshipKeys => field ??= new List<object>
    {
        // For relationships looking for files in this folder
        $"{ParentPrefix}{ParentId}"
    };

    #endregion


    /// <inheritdoc cref="IPageModel.Id"/>
    public int Id { get; init; }

    /// <inheritdoc />
    public int ParentId { get; init; }

    /// <inheritdoc cref="IPageModel.Guid"/>
    public Guid Guid { get; init; }

    /// <inheritdoc cref="IPageModel.Title"/>
    public string? Title { get; init; }

    /// <inheritdoc />
    public string? Name { get; init; }

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
    public string? LinkTarget { get; init; }


    /// <inheritdoc />
    public string? Path { get; init; }

    /// <inheritdoc />
    public string? Url { get; init; }

    /// <inheritdoc cref="IPageModel.Created" />
    public DateTime Created { get; init; }

    /// <inheritdoc cref="IPageModel.Modified" />
    public DateTime Modified { get; init; }

    /// <inheritdoc />
    public bool IsDeleted { get; init; }

    // Not implemented, and not sure if we should, since it would potentially introduce a lot of prefetch data
    IEnumerable<IPageModel> IPageModel.Children => throw new NotImplementedException();

}