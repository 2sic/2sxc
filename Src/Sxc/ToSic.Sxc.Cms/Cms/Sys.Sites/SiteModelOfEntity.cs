using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Sites.Internal;

public class SiteModelOfEntity: ModelFromEntity, ISiteModel
{
    public int Id => _entity.EntityId;
    public Guid Guid => _entity.EntityGuid;
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;
    public string? Name => GetThis<string>(null);
    public string? Url => GetThis<string>(null);
    public string? Languages => GetThis<string>(null);
    public string? DefaultLanguage => GetThis<string>(null);
    public int ZoneId => GetThis(0);
    public int ContentAppId => GetThis(0);
    public int PrimaryAppId => GetThis(0);
}