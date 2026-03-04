using ToSic.Sxc.Services.Cache.Sys.CacheKey;
using ToSic.Sys.Users;

namespace ToSic.Sxc.ServicesTests.CacheTests;
public class CacheKeyForElevationTests
{
    #region Setup / Helpers

    private const int OneMinute = 60;
    private const int TwoMinutes = 120;

    private static List<UserElevation> UserElevations =>
        Enum.GetValues(typeof(UserElevation)).Cast<UserElevation>().ToList();

    private static Dictionary<UserElevation, int> GetElevationsForOne(UserElevation elevation, int time) =>
        new() { { elevation, time } };

    private void AllMatchIsEnabledFor(Dictionary<UserElevation, int> dic, IEnumerable<UserElevation> elevations, bool expected)
    {
        foreach (var elevation in elevations)
            Equal(expected, dic.IsEnabledFor(elevation));
    }

    private void AllMatchIsEnabledForExact(Dictionary<UserElevation, int> dic, IEnumerable<UserElevation> elevations, bool? expected)
    {
        foreach (var elevation in elevations)
            Equal(expected, dic.IsEnabledForExact(elevation));
    }

    #endregion

    [Fact]
    public void EmptyEnabledForExactAlwaysNull() =>
        AllMatchIsEnabledForExact(new(), UserElevations, null);

    [Fact]
    public void EmptyEnabledForAlwaysFalse() =>
        AllMatchIsEnabledFor(new(), UserElevations, false);

    [Fact]
    public void ForSystemAdminTrueForSystemAdmin()
    {
        var dic = GetElevationsForOne(UserElevation.SystemAdmin, OneMinute);
        True(dic.IsEnabledFor(UserElevation.SystemAdmin));
    }

    [Fact]
    public void ForSystemAdminFalseForAllOthers() =>
        AllMatchIsEnabledFor(
            GetElevationsForOne(UserElevation.SystemAdmin, OneMinute),
            UserElevations.Where(e => e != UserElevation.SystemAdmin),
            false
        );

    [Fact]
    public void SetExactSystemAdminEqualsForSystemAdmin()
    {
        var dic = GetElevationsForOne(UserElevation.SystemAdmin, OneMinute);
        var empty = new Dictionary<UserElevation, int>()
            .SetOne(UserElevation.SystemAdmin, OneMinute);
        Equal(empty, dic);
    }

    [Fact]
    public void SetExact2X()
    {
        var dic = GetElevationsForOne(UserElevation.SystemAdmin, OneMinute);
        dic.Add(UserElevation.ContentEdit, TwoMinutes);
        var empty = new Dictionary<UserElevation, int>()
            .SetOne(UserElevation.SystemAdmin, OneMinute)
            .SetOne(UserElevation.ContentEdit, TwoMinutes);
        Equal(empty, dic);
    }

    [Theory]
    [InlineData(CacheKeyConfig.Disabled, false)]
    [InlineData(CacheKeyConfig.EnabledWithoutTime, true)]
    [InlineData(OneMinute, true)]
    public void ResetAll(int duration, bool expected) =>
        AllMatchIsEnabledFor(ForElevationExtensions.ResetAll(duration), UserElevations, expected);

    [Fact]
    public void EnabledForAllExceptSystemAdmins()
    {
        var dic = ForElevationExtensions.ResetAll(OneMinute)
            .SetOne(UserElevation.SystemAdmin, CacheKeyConfig.Disabled);

        AllMatchIsEnabledFor(dic, UserElevations.Where(e => e != UserElevation.SystemAdmin), true);
        False(dic.IsEnabledFor(UserElevation.SystemAdmin));
    }

    [Theory]
    [InlineData(UserElevation.SystemAdmin,  false, true)]
    [InlineData(UserElevation.ContentEdit,  false, true)]
    [InlineData(UserElevation.Anonymous,   false,  true)]
    [InlineData(UserElevation.All,  false, false)]
    [InlineData(UserElevation.Unknown,  false, false)]
    public void SetOneOrAll(UserElevation elevation, bool forOne, bool forRest)
    {
        var dic = ForElevationExtensions.ResetAll(OneMinute)
            .SetForOneOrAll(elevation, CacheKeyConfig.Disabled);

        AllMatchIsEnabledFor(dic, [elevation], forOne);
        AllMatchIsEnabledFor(dic, UserElevations.Where(e => e != elevation), forRest);
    }

    [Fact]
    public void DisableRangeEditToSysAdmin()
    {
        var dic = ForElevationExtensions.ResetAll(OneMinute)
            .SetRange(UserElevation.ContentEdit, UserElevation.SystemAdmin, CacheKeyConfig.Disabled);
        AllMatchIsEnabledFor(dic, UserElevations.Where(e => e < UserElevation.ContentEdit), true);
        AllMatchIsEnabledFor(dic, UserElevations.Where(e => e >= UserElevation.ContentEdit), false);
    }

    [Fact]
    public void DisableRangeInvalid() =>
        Throws<ArgumentOutOfRangeException>(() =>
            ForElevationExtensions.ResetAll(OneMinute)
                .SetRange(UserElevation.ContentEdit, UserElevation.Anonymous, CacheKeyConfig.Disabled)
        );
}
