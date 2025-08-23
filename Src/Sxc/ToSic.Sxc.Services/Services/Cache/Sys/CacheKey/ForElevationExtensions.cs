using ToSic.Sxc.Cms.Users;

namespace ToSic.Sxc.Services.Cache.Sys.CacheKey;
public static class ForElevationExtensions
{
    //public static int GetSlidingAny(this CacheKeyConfig keyConfig)
    //    => keyConfig.ForElevation.TryGetValue(UserElevation.Any, out var time) && time > 0 ? time : 0;

    public static Dictionary<UserElevation, int> ResetAll(int seconds) =>
        new() { [UserElevation.All] = seconds };

    public static Dictionary<UserElevation, int> SetForOneOrAll(this IDictionary<UserElevation, int> dic, UserElevation elevation, int seconds) =>
        // If not specified, for all, or within the lowest and highest elevation, then disable
        elevation is UserElevation.Unknown or UserElevation.All
            ? ResetAll(seconds)
            : SetOne(dic, elevation, seconds);

    public static Dictionary<UserElevation, int> SetOne(this IDictionary<UserElevation, int> dic, UserElevation elevation, int seconds) =>
        new(dic)
        {
            [elevation] = seconds
        };

    public static bool IsEnabledFor(this IDictionary<UserElevation, int> dic, UserElevation elevation) =>
        IsEnabledForExact(dic, elevation)
        ?? IsEnabledForExact(dic, UserElevation.All)
        ?? false;

    public static bool? IsEnabledForExact(this IDictionary<UserElevation, int> dic, UserElevation elevation) =>
        dic.TryGetValue(elevation, out var time)
            ? time > CacheKeyConfig.Disabled
            : null; // true if 0 or greater, false if -1

    public static int SecondsFor(this IDictionary<UserElevation, int> dic, UserElevation elevation)
        => SecondsForExact(dic, elevation)
           ?? SecondsForExact(dic, UserElevation.All)
           ?? 0;

    public static int? SecondsForExact(this IDictionary<UserElevation, int> dic, UserElevation elevation) =>
        dic.TryGetValue(elevation, out var time) && time > 0
            ? time
            : null; // true if 0 or greater, false if -1
}
