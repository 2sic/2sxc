using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Users.Sys;


internal class UserModelOfEntity : ModelFromEntity, IUserModel
{

    public string? Email => GetThis<string>(null);

    public int Id => _entity.EntityId;

    public Guid Guid => _entity.EntityGuid;

    public DateTime Created => _entity.Created;

    public DateTime Modified => _entity.Modified;

    public bool IsAnonymous => GetThis(false);

    public bool IsSiteAdmin => GetThis(false);

    public bool IsContentAdmin => GetThis(false);

    public bool IsContentEditor => GetThis(false);

    public string? NameId => GetThis<string>(null);

    public bool IsSystemAdmin => GetThis(false);

    public bool IsSiteDeveloper => GetThis(false);

    //IMetadata ICmsUser.Metadata => null;

    public string? Name => GetThis<string>(null);

    public string? Username => GetThis<string>(null);

    //IMetadataOf IHasMetadata.Metadata => null;

    public IEnumerable<IUserRoleModel> Roles => AsList<UserRoleModelOfEntity>(_entity.Children(field: nameof(Roles)))!;

}