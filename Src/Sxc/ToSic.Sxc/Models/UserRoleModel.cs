using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

public class UserRoleModel: DataModel, IUserRoleModel
{
    public string Name { get; init; }
    public int Id => _entity.EntityId;
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;

}