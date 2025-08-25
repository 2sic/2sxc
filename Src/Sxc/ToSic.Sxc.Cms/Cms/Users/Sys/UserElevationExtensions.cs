using ToSic.Sxc.Context;
using ToSic.Sys.Users;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Cms.Users.Sys;

public static class UserElevationExtensions
{
    public static UserElevation GetElevation(this IUser user)
    {
        if (user.IsAnonymous) return UserElevation.Anonymous;
        if (user.IsSystemAdmin) return UserElevation.SystemAdmin;
        if (user.IsSiteAdmin) return UserElevation.SiteAdmin;
        if (user.IsContentAdmin) return UserElevation.ContentAdmin;
        if (user.IsContentEditor) return UserElevation.ContentEdit;
        return UserElevation.View;
    }

    public static UserElevation GetElevation(this ICmsUser user)
    {
        var underlyingUser = ((Wrapper<IUser>)user).GetContents()!;
        return underlyingUser.GetElevation();
    }

    private static bool IsAtLeast(this UserElevation elevation, UserElevation minimum)
        => elevation >= minimum;

    private static bool IsAtMost(this UserElevation elevation, UserElevation maximum)
        => elevation <= maximum;

    public static bool IsForAllOrInRange(this UserElevation user, UserElevation minimum, UserElevation maximum)
        => (minimum <= UserElevation.All || user.IsAtLeast(minimum)) &&
           (maximum <= UserElevation.All || user.IsAtMost(maximum));
}