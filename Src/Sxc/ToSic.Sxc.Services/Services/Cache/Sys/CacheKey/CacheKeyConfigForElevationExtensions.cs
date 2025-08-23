using ToSic.Sxc.Cms.Users;

namespace ToSic.Sxc.Services.Cache.Sys.CacheKey;
public static class CacheKeyConfigForElevationExtensions
{
    public static int GetSlidingAny(this CacheKeyConfig keyConfig)
        => keyConfig.ForElevation.TryGetValue(UserElevation.Any, out var time) && time > 0 ? time : 0;

    public static CacheKeyConfig SetElevation(this CacheKeyConfig keyConfig, UserElevation elevation, int seconds = -1)
    {
        var elevationCopy = new Dictionary<UserElevation, int>(keyConfig.ForElevation)
        {
            [elevation] = seconds
        };
        return keyConfig with { ForElevation = elevationCopy };
    }

    public static CacheKeyConfig SetDisabled(this CacheKeyConfig keyConfig, UserElevation elevation)
        => SetElevation(keyConfig, elevation, CacheKeyConfig.Disabled);

    public static bool IsEnabledFor(this CacheKeyConfig keyConfig, UserElevation elevation) =>
        IsEnabledForExact(keyConfig, elevation)
        ?? IsEnabledForExact(keyConfig, UserElevation.Any)
        ?? false;

    public static bool? IsEnabledForExact(this CacheKeyConfig keyConfig, UserElevation elevation) =>
        keyConfig.ForElevation.TryGetValue(elevation, out var time)
            ? time > CacheKeyConfig.Disabled
            : null; // true if 0 or greater, false if -1

    public static int SecondsFor(this CacheKeyConfig keyConfig, UserElevation elevation)
        => SecondsForExact(keyConfig, elevation)
           ?? SecondsForExact(keyConfig, UserElevation.Any)
           ?? 0;

    public static int? SecondsForExact(this CacheKeyConfig keyConfig, UserElevation elevation) =>
        keyConfig.ForElevation.TryGetValue(elevation, out var time) && time > 0
            ? time
            : null; // true if 0 or greater, false if -1
}
