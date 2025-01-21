using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data;

internal class Lifecycle(IEntity entity, IUserService userService): ILifecycle
{
    public int Version => entity.Version;
    public DateTime Created => entity.Created;
    public DateTime Modified => entity.Modified;
    public int OwnerUserId => entity.OwnerId;
    public IUserModel OwnerUser => null; // userService.Get(entity.OwnerId);
}