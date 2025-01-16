using ToSic.Sxc.Data.Model;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

public class UserRoleModel: DataModel, IUserRoleModelSync, IUserRoleModel
{
    public int Id => _entity.EntityId;
    public string Name => _entity.Get<string>(nameof(Name));
    public DateTime Created => _entity.Created;
    public DateTime Modified => _entity.Modified;

}