using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Pages.Sys;

public class PageModelOfEntity: ModelFromEntity, IPageModel
{
    public int Id => _entity.EntityId;
    public int ParentId => GetThis(0);
    public Guid Guid => _entity.EntityGuid;
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
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;
    public bool IsDeleted => GetThis(false);

    public IEnumerable<IPageModel> Children => AsList<PageModelOfEntity>(_entity.Children(field: nameof(Children)))!;
}