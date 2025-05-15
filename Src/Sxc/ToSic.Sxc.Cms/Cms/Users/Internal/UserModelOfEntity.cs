using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Users.Internal;


internal class UserModelOfEntity : ModelFromEntity, IUserModel
{

    public string Email => _entity.Get<string>(nameof(Email));

    public int Id => _entity.EntityId;

    public Guid Guid => _entity.EntityGuid;

    public DateTime Created => _entity.Created;

    public DateTime Modified => _entity.Modified;

    public bool IsAnonymous => _entity.Get<bool>(nameof(IsAnonymous));

    public bool IsSiteAdmin => _entity.Get<bool>(nameof(IsSiteAdmin));

    public bool IsContentAdmin => _entity.Get<bool>(nameof(IsContentAdmin));

    public bool IsContentEditor => _entity.Get<bool>(nameof(IsContentEditor));

    public string NameId => _entity.Get<string>(nameof(NameId));

    public bool IsSystemAdmin => _entity.Get<bool>(nameof(IsSystemAdmin));

    public bool IsSiteDeveloper => _entity.Get<bool>(nameof(IsSiteDeveloper));

    //IMetadata ICmsUser.Metadata => null;

    public string Name => _entity.Get<string>(nameof(Name));

    public string Username => _entity.Get<string>(nameof(Username));

    //IMetadataOf IHasMetadata.Metadata => null;

    public IEnumerable<IUserRoleModel> Roles => AsList<UserRoleModelOfEntity>(_entity.Children(field: nameof(Roles)));

}