using ToSic.Sxc.Data.Model;

namespace ToSic.Sxc.Models.Internal;

public class UserRoleModelOfEntity: DataModel, IUserRoleModelSync, IUserRoleModel
{
    public int Id => _entity.EntityId;
    public string Name => _entity.Get<string>(nameof(Name));
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;

}