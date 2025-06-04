using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;

namespace ToSic.Sxc.Cms.Sites.Internal;

/// <summary>
/// Internal class to hold all the information about the site,
/// until it's converted to an IEntity in the <see cref="Sites"/> DataSource.
/// 
/// TODO:
/// </summary>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentTypeSpecs(
    Guid = "89ef9f2c-98d3-42e9-a190-b9b3fc814284",
    Description = "Site information",
    Name = TypeName
)]
public record SiteModel: IRawEntity, ISiteModel
{
    #region IRawEntity

    internal const string TypeName = "Site";

    internal static DataFactoryOptions Options => new()
    {
        AutoId = false,
        TypeName = TypeName,
        TitleField = nameof(Name),
    };

    IDictionary<string, object> IRawEntity.Attributes(RawConvertOptions options) => new Dictionary<string, object>
    {
        { nameof(Name), Name },
        { nameof(Url), Url },
        { nameof(Languages), Languages },
        { nameof(DefaultLanguage), DefaultLanguage },
        { nameof(ZoneId), ZoneId },
        { nameof(ContentAppId), ContentAppId },
        { nameof(PrimaryAppId), PrimaryAppId },
    };

    #endregion

    /// <inheritdoc cref="ISiteModel.Id" />
    public int Id { get; init; }

    /// <inheritdoc cref="ISiteModel.Guid" />
    public Guid Guid { get; init; }

    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public string Url { get; init; }

    /// <inheritdoc />
    public string Languages { get; init; }

    /// <inheritdoc />
    public string DefaultLanguage { get; init; }

    /// <inheritdoc cref="ISiteModel.Created" />
    public DateTime Created { get; init; }

    /// <inheritdoc cref="ISiteModel.Modified" />
    public DateTime Modified { get; init; }


    /// <inheritdoc />
    public int ZoneId { get; init; }

    /// <inheritdoc />
    public int ContentAppId { get; init; }

    /// <inheritdoc />
    public int PrimaryAppId { get; init; }

}