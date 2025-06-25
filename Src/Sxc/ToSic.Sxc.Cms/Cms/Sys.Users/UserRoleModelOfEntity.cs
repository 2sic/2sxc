using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Cms.Users.Internal;

public class UserRoleModelOfEntity: ModelFromEntity, IUserRoleModel
{
    public int Id => _entity.EntityId;
    public string Name => _entity.Get<string>(nameof(Name)) ?? "unknown";
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;

}