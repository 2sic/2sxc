using ToSic.Sxc.Context.Internal;
using static ToSic.Sxc.Tests.ContextTests.ParametersTestData;

namespace ToSic.Sxc.Tests.ContextTests;


public class ParametersToStringTests
{
    [Fact]
    public void TestMethod1()
    {
    }

    #region Very Basic Tests - ToString etc.

    [Fact]
    public void ParamsToStringIdSort()
        => Equal(Id27SortDescending, ParametersId27SortDescending().ToString());

    [Fact]
    public void ParamsToStringSortId()
        => Equal(SortDescendingId27, ParametersSortDescendingId27().Prioritize("sort").ToString());

    [Fact]
    public void ParamsToStringSortIdDifferentCasing()
        => Equal(SortDescendingId27, ParametersSortDescendingId27().Prioritize("SORT").ToString());

    #endregion

    #region Enforce Parameter Sort!

    
    /// <summary>
    /// enforce sorting of the parameters for lightspeed use
    /// </summary>
    [Fact]
    public void ParamsToStringSortIdForceSorted()
        => Equal(Id27SortDescending, ((Parameters)ParametersSortDescendingId27()).ToString(sort: true));

    [Fact]
    public void ParamsToStringSortIdForceNotSorted()
        => Equal(SortDescendingId27, ((Parameters)ParametersSortDescendingId27()).ToString(sort: false));

    #endregion

}