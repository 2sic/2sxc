using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToSic.Sxc.Context.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Tests.ContextTests.ParametersTestData;

namespace ToSic.Sxc.Tests.ContextTests;

[TestClass]
public class ParametersAccessedTests
{

    #region Get - with access tracking new v17.09+

    [TestMethod]
    public void AccessedKeysCountNone()
        => AreEqual(0, ((Parameters)ParametersId27SortDescending()).UsedKeys.Count);

    [TestMethod]
    [DataRow(0, "")]
    [DataRow(0, ",")]
    [DataRow(0, "unknown")]
    [DataRow(1, "id")]
    [DataRow(1, "id,ID")]
    [DataRow(1, "id,unknown")]
    [DataRow(2, "id,sort")]
    public void AccessedKeysCount(int count, string keysCsv)
    {
        var p = ParametersId27SortDescending();
        var keys = keysCsv?.Split(',') ?? [];
        foreach (var key in keys) p.Get(key);
        AreEqual(count, ((Parameters)p).UsedKeys.Count);
    }

    [TestMethod]
    [DataRow(null, "")]
    [DataRow(null, ",")]
    [DataRow(null, "unknown")]
    [DataRow("id", "id")]
    [DataRow("id", "ID")]
    [DataRow("id", "id,ID")]
    [DataRow("id", "ID,id")]
    [DataRow("id", "id,unknown")]
    [DataRow("id,sort", "id,sort")]
    [DataRow("id,sort", "id,sort,ID,unknown")]
    public void AccessedKeysList(string accessedCsv, string keysCsv)
    {
        var p = ParametersId27SortDescending();
        var keys = keysCsv?.Split(',') ?? [];
        var accessed = accessedCsv?.Split(',') ?? [];
        foreach (var key in keys) p.Get(key);
        AreEqual(accessed.Length, ((Parameters)p).UsedKeys.Count);
        CollectionAssert.AreEqual(accessed, ((Parameters)p).UsedKeys.ToArray());
    }

    #endregion
}