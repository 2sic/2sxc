using ToSic.Sxc.Data.Model;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// Site model for entities returned by the <see cref="Sites"/> DataSource.
/// </summary>
/// <remarks>
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn Site](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Portals.PortalInfo.html)
/// * [Oqtane Site](https://docs.oqtane.org/api/Oqtane.Models.Site.html)
///
/// History
/// 
/// * Introduced in v19.01
/// </remarks>
[DataModelConversion(Map = [
    typeof(DataModelFrom<IEntity, ISiteModelSync, SiteModel>),
])]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface ISiteModel : IDataModel
{
    /// <inheritdoc cref="ISiteModelSync.Id" />
    int Id { get; }

    /// <inheritdoc cref="ISiteModelSync.Guid" />
    Guid Guid { get; }

    /// <inheritdoc cref="ISiteModelSync.Created" />
    DateTime Created { get; }

    /// <inheritdoc cref="ISiteModelSync.Modified" />
    DateTime Modified { get; }

    /// <inheritdoc cref="ISiteModelSync.Name" />
    string Name { get; }

    /// <inheritdoc cref="ISiteModelSync.Url" />
    string Url { get; }

    /// <inheritdoc cref="ISiteModelSync.Languages" />
    string Languages { get; }

    /// <inheritdoc cref="ISiteModelSync.DefaultLanguage" />
    string DefaultLanguage { get; }

    /// <inheritdoc cref="ISiteModelSync.ZoneId" />
    int ZoneId { get; }

    /// <inheritdoc cref="ISiteModelSync.ContentAppId" />
    int ContentAppId { get; }

    /// <inheritdoc cref="ISiteModelSync.PrimaryAppId" />
    int PrimaryAppId { get; }
}