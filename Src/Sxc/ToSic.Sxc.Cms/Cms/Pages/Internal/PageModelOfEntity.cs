using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Pages.Internal;

public class PageModelOfEntity: ModelFromEntity, IPageModel
{
    public int Id => _entity.EntityId;
    public int ParentId => _entity.Get<int>(nameof(ParentId));
    public Guid Guid => _entity.EntityGuid;
    public string Title => _entity.Get<string>(nameof(Title));
    public string Name => _entity.Get<string>(nameof(Name));
    public bool IsClickable => _entity.Get<bool>(nameof(IsClickable));
    public int Order => _entity.Get<int>(nameof(Order));
    public bool IsNavigation => _entity.Get<bool>(nameof(IsNavigation));
    public bool HasChildren => _entity.Get<bool>(nameof(HasChildren));
    public int Level => _entity.Get<int>(nameof(Level));
    public string LinkTarget => _entity.Get<string>(nameof(LinkTarget));
    public string Path => _entity.Get<string>(nameof(Path));
    public string Url => _entity.Get<string>(nameof(Url));
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;
    public bool IsDeleted => _entity.Get<bool>(nameof(IsDeleted));

    public IEnumerable<IPageModel> Children => AsList<PageModelOfEntity>(_entity.Children(field: nameof(Children)));
}