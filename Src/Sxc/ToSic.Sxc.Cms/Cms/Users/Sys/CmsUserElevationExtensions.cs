using ToSic.Sxc.Context;
using ToSic.Sys.Users;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Cms.Users.Sys;

public static class CmsUserElevationExtensions
{
    public static UserElevation GetElevation(this ICmsUser user)
    {
        var underlyingUser = ((Wrapper<IUser>)user).GetContents()!;
        return underlyingUser.GetElevation();
    }
}