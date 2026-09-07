using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
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
[ContentTypeUse(Type = typeof(IPageModel))]
public record PageModelRaw: IPageModel, IRawEntityConvertible
{
    private const string ParentPrefix = "ParentId:";

    /// <inheritdoc cref="IPageModel.Id"/>
    public int Id { get; init; }

    /// <inheritdoc />
    public int ParentId { get; init; }

    /// <inheritdoc cref="IPageModel.Guid"/>
    public Guid Guid { get; init; }

    /// <inheritdoc cref="IPageModel.Title"/>
    [ContentTypeTitle]
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
    IRawEntityConverter IRawEntityConvertible.GetConverter() => Converter;

    private static IRawEntityConverter Converter { get; } =
        new RawEntityConverterFactory<PageModelRaw>((source, _) => new RawEntity
        {
            Id = source.Id,
            Guid = source.Guid,
            Created = source.Created,
            Modified = source.Modified,
            Values = new Dictionary<string, object?>
            {
                { nameof(Title), source.Title },
                { nameof(Name), source.Name },
                { nameof(ParentId), source.ParentId },
                { nameof(IsNavigation), source.IsNavigation },
                { nameof(Path), source.Path },
                { nameof(Url), source.Url },
                { nameof(IsClickable), source.IsClickable },
                { nameof(Order), source.Order },
                { nameof(IsDeleted), source.IsDeleted },
                { nameof(Level), source.Level },
                { nameof(HasChildren), source.HasChildren },
                { nameof(LinkTarget), source.LinkTarget },
                { nameof(IPageModel.Children), new RawRelationship { Keys = [$"{ParentPrefix}{source.Id}"] } }
            },
            RelationshipKeys = [$"{ParentPrefix}{source.ParentId}"]
        });
}
