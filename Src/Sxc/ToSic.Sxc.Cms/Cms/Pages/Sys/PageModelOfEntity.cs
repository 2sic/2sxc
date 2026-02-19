namespace ToSic.Sxc.Cms.Pages.Sys;

public record PageModelOfEntity: ModelOfEntity, IPageModel
{
    public int Id => Entity.EntityId;
    public int ParentId => GetThis(0);
    public Guid Guid => Entity.EntityGuid;
    public string? Title => GetThis<string>(null);
    public string? Name => GetThis<string>(null);
    public bool IsClickable => GetThis(false);
    public int Order => GetThis(0);
    public bool IsNavigation => GetThis(false);
    public bool HasChildren => GetThis(false);
    public int Level => GetThis(0);
    public string? LinkTarget => GetThis<string>(null);
    public string? Path => GetThis<string>(null);
    public string? Url => GetThis<string>(null);
    public DateTime Created => Entity.Created;
    public DateTime Modified => Entity.Modified;
    public bool IsDeleted => GetThis(false);

    public IEnumerable<IPageModel> Children =>
        Entity.Children(field: nameof(Children)).AsList<PageModelOfEntity>();
}