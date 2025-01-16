using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Internal;
using ToSic.Eav.Data.Raw;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Models.Internal;

/// <summary>
/// Internal class to hold all the information about the site,
/// until it's converted to an IEntity in the <see cref="Sites"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn Site](TODO:https://docs.dnncommunity.org/api/DotNetNuke.Entities.Portals.PortalInfo.html)
/// * [Oqtane Site](TODO:https://docs.oqtane.org/api/Oqtane.Models.Sites.html)
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
    Guid = "89ef9f2c-98d3-42e9-a190-b9b3fc814284",
    Description = "User-Role in the site",
    Name = TypeName
)]
public class SiteDataRaw: IRawEntity, ISiteModelSync
{
    internal const string TypeName = "Site";

    internal static DataFactoryOptions Options => new()
    {
        AutoId = false,
        TypeName = TypeName,
        TitleField = nameof(Name),
    };

    /// <inheritdoc cref="ISiteModelSync.Id" />
    public int Id { get; init; }

    /// <inheritdoc cref="ISiteModelSync.Guid" />
    public Guid Guid { get; init; }

    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public string Url { get; init; }

    /// <inheritdoc />
    public string Languages { get; init; }

    /// <inheritdoc />
    public string DefaultLanguage { get; init; }

    /// <inheritdoc cref="ISiteModelSync.Created" />
    public DateTime Created { get; init; }

    /// <inheritdoc cref="ISiteModelSync.Modified" />
    public DateTime Modified { get; init; }


    /// <inheritdoc />
    public int ZoneId { get; init; }

    /// <inheritdoc />
    public int ContentAppId { get; init; }

    /// <inheritdoc />
    public int PrimaryAppId { get; init; }

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    public IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>
    {
        { nameof(Name), Name },
        { nameof(Url), Url },
        { nameof(Languages), Languages },
        { nameof(DefaultLanguage), DefaultLanguage },
        { nameof(ZoneId), ZoneId },
        { nameof(ContentAppId), ContentAppId },
        { nameof(PrimaryAppId), PrimaryAppId },
    };

}