using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Context.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Tests.ContextTests.ParametersTestData;

namespace ToSic.Sxc.Tests.ContextTests;

[TestClass]
public class ParametersToStringTests
{
    [TestMethod]
    public void TestMethod1()
    {
    }

    #region Very Basic Tests - ToString etc.

    [TestMethod]
    public void ParamsToStringIdSort()
        => AreEqual(Id27SortDescending, ParametersId27SortDescending().ToString());

    [TestMethod]
    public void ParamsToStringSortId()
        => AreEqual(SortDescendingId27, ParametersSortDescendingId27().Prioritize("sort").ToString());

    [TestMethod]
    public void ParamsToStringSortIdDifferentCasing()
        => AreEqual(SortDescendingId27, ParametersSortDescendingId27().Prioritize("SORT").ToString());

    #endregion

    #region Enforce Parameter Sort!

    
    /// <summary>
    /// enforce sorting of the parameters for lightspeed use
    /// </summary>
    [TestMethod]
    public void ParamsToStringSortIdForceSorted()
        => AreEqual(Id27SortDescending, ((Parameters)ParametersSortDescendingId27()).ToString(sort: true));

    [TestMethod]
    public void ParamsToStringSortIdForceNotSorted()
        => AreEqual(SortDescendingId27, ((Parameters)ParametersSortDescendingId27()).ToString(sort: false));

    #endregion

}