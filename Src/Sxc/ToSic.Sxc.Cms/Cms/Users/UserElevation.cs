using ToSic.Sxc.Context;
using ToSic.Sys.Users;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Cms.Users;

[WorkInProgressApi("v20.01")]
public enum UserElevation
{
    Unknown = 0,

    Any = 1,

    Anonymous = 10,

    View = 20,

    // CreateDraft = 3,

    // EditDraft = 4,

    // Create = 5,

    ContentEdit = 60,

    ContentAdmin = 70,

    SiteAdmin = 80,

    SystemAdmin = 99,
}

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
        => (minimum <= UserElevation.Any || user.IsAtLeast(minimum)) &&
           (maximum <= UserElevation.Any || user.IsAtMost(maximum));
}