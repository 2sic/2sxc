namespace ToSic.Sxc.Cms.Sites.Sys;

public record SiteModelOfEntity: ModelOfEntity, ISiteModel
{
    public int Id => Entity.EntityId;
    public Guid Guid => Entity.EntityGuid;
    public DateTime Created => Entity.Created;
    public DateTime Modified => Entity.Modified;
    public string? Name => GetThis<string>(null);
    public string? Url => GetThis<string>(null);
    public string? Languages => GetThis<string>(null);
    public string? DefaultLanguage => GetThis<string>(null);
    public int ZoneId => GetThis(0);
    public int ContentAppId => GetThis(0);
    public int PrimaryAppId => GetThis(0);
}