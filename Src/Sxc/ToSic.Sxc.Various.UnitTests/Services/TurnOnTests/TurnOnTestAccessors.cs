using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests.TurnOnTests;

internal static class TurnOnTestAccessors
{
    public static object TacPickOrBuildSpecs(object runOrSpecs, object require = default, object data = default, object[] args = default, string addContext = default)
        => TurnOnService.PickOrBuildSpecs(runOrSpecs, require, data, args, addContext);
}