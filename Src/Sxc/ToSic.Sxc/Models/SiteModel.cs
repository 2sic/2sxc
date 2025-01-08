using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// Site model for entities returned by the <see cref="Sites"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn Site](https://docs.dnncommunity.org/api/DotNetNuke.Entities.Portals.PortalInfo.html)
/// * [Oqtane Site](https://docs.oqtane.org/api/Oqtane.Models.Site.html)
/// 
/// </summary>
/// <remarks>
/// * Introduced in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class SiteModel: DataModel, ISiteModel
{
    /// <inheritdoc />
    public int Id => _entity.EntityId;
    /// <inheritdoc />
    public Guid Guid => _entity.EntityGuid;
    /// <inheritdoc />
    public string Name => _entity.Get<string>(nameof(Name));
    /// <inheritdoc />
    public string Url => _entity.Get<string>(nameof(Url));
    /// <inheritdoc />
    public string Languages => _entity.Get<string>(nameof(Languages));
    /// <inheritdoc />
    public string DefaultLanguage => _entity.Get<string>(nameof(DefaultLanguage));
    /// <inheritdoc />
    public DateTime Created => _entity.Get<DateTime>(nameof(Created));
    /// <inheritdoc />
    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));
    /// <inheritdoc />
    public int ZoneId => _entity.Get<int>(nameof(ZoneId));
    /// <inheritdoc />
    public int ContentAppId => _entity.Get<int>(nameof(ContentAppId));
    /// <inheritdoc />
    public int PrimaryAppId => _entity.Get<int>(nameof(PrimaryAppId));
}