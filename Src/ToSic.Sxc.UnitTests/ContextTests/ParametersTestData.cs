using ToSic.Sxc.Context;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.ContextTests;

internal class ParametersTestData
{
    /// <summary>
    /// Get TestParameters with id=27 and sort=descending
    /// </summary>
    internal static IParameters ParametersId27SortDescending() => NewParameters(new()
    {
        { "id", "27" },
        { "sort", "descending" }
    });

    internal const string Id27SortDescending = "id=27&sort=descending";

    /// <summary>
    /// Get TestParameters with sort=descending and id=27 (so added in Z-A order)
    /// </summary>
    internal static IParameters ParametersSortDescendingId27() => NewParameters(new()
    {
        { "sort", "descending" },
        { "id", "27" },
    });

    internal const string SortDescendingId27 = "sort=descending&id=27";

}