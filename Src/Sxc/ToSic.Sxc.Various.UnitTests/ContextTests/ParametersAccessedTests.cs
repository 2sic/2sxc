using ToSic.Sxc.Context.Sys;
using static ToSic.Sxc.Tests.ContextTests.ParametersTestData;

namespace ToSic.Sxc.Tests.ContextTests;


public class ParametersAccessedTests
{

    #region Get - with access tracking new v17.09+

    [Fact]
    public void AccessedKeysCountNone()
        => Equal(0, ((Parameters)ParametersId27SortDescending()).UsedKeys.Count);

    [Theory]
    [InlineData(0, "")]
    [InlineData(0, ",")]
    [InlineData(0, "unknown")]
    [InlineData(1, "id")]
    [InlineData(1, "id,ID")]
    [InlineData(1, "id,unknown")]
    [InlineData(2, "id,sort")]
    public void AccessedKeysCount(int count, string keysCsv)
    {
        var p = ParametersId27SortDescending();
        var keys = keysCsv?.Split(',') ?? [];
        foreach (var key in keys) p.Get(key);
        Equal(count, ((Parameters)p).UsedKeys.Count);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(null, ",")]
    [InlineData(null, "unknown")]
    [InlineData("id", "id")]
    [InlineData("id", "ID")]
    [InlineData("id", "id,ID")]
    [InlineData("id", "ID,id")]
    [InlineData("id", "id,unknown")]
    [InlineData("id,sort", "id,sort")]
    [InlineData("id,sort", "id,sort,ID,unknown")]
    public void AccessedKeysList(string accessedCsv, string keysCsv)
    {
        var p = ParametersId27SortDescending();
        var keys = keysCsv?.Split(',') ?? [];
        var accessed = accessedCsv?.Split(',') ?? [];
        foreach (var key in keys) p.Get(key);
        Equal(accessed.Length, ((Parameters)p).UsedKeys.Count);
        Equal(accessed, ((Parameters)p).UsedKeys.ToArray());
    }

    #endregion
}