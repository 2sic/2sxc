using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Sites.Internal;

public class SiteModelOfEntity: ModelFromEntity, ISiteModel
{
    public int Id => _entity.EntityId;
    public Guid Guid => _entity.EntityGuid;
    public DateTime Created => _entity.Get<DateTime>(nameof(Created));
    public DateTime Modified => _entity.Get<DateTime>(nameof(Modified));
    public string Name => _entity.Get<string>(nameof(Name));
    public string Url => _entity.Get<string>(nameof(Url));
    public string Languages => _entity.Get<string>(nameof(Languages));
    public string DefaultLanguage => _entity.Get<string>(nameof(DefaultLanguage));
    public int ZoneId => _entity.Get<int>(nameof(ZoneId));
    public int ContentAppId => _entity.Get<int>(nameof(ContentAppId));
    public int PrimaryAppId => _entity.Get<int>(nameof(PrimaryAppId));
}