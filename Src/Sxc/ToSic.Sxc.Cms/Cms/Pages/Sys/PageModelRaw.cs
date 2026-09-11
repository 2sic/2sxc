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
        new RawEntityConverterFactory<PageModelRaw>((source, _) => new PageRaw(source)
        {
            Id = source.Id,
            Guid = source.Guid,
            Created = source.Created,
            Modified = source.Modified,
            RelationshipKeys = [$"{ParentPrefix}{source.ParentId}"]
        });

    private sealed record PageRaw(PageModelRaw Source) : RawEntity
    {
        protected override IDictionary<string, object?> GetValues() =>
            new Dictionary<string, object?>
            {
                { nameof(Title), Source.Title },
                { nameof(Name), Source.Name },
                { nameof(ParentId), Source.ParentId },
                { nameof(IsNavigation), Source.IsNavigation },
                { nameof(Path), Source.Path },
                { nameof(Url), Source.Url },
                { nameof(IsClickable), Source.IsClickable },
                { nameof(Order), Source.Order },
                { nameof(IsDeleted), Source.IsDeleted },
                { nameof(Level), Source.Level },
                { nameof(HasChildren), Source.HasChildren },
                { nameof(LinkTarget), Source.LinkTarget },
                { nameof(IPageModel.Children), new RawRelationship { Keys = [$"{ParentPrefix}{Source.Id}"] } }
            };
    }
}
