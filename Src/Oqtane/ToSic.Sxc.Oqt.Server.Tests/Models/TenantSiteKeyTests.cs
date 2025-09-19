using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Models.Tests;

public class TenantSiteKeyTests
{
    [Fact]
    public void Equality_Works_For_Same_Values()
    {
        var a = new TenantSiteKey(3, 7);
        var b = new TenantSiteKey(3, 7);
        True(a.Equals(b));
        Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Inequality_Works_For_Different_Values()
    {
        var a = new TenantSiteKey(3, 7);
        var b = new TenantSiteKey(4, 7);
        var c = new TenantSiteKey(3, 8);
        NotEqual(a, b);
        NotEqual(a, c);
    }

    [Fact]
    public void ToString_Is_Deterministic()
    {
        var key = new TenantSiteKey(3, 7);
        Equal("t:3/s:7", key.ToString());
    }
}
