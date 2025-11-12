using ToSic.Sxc.Backend.App;

#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test accessor methods for ExportExtension to isolate test usage from production code references.
/// These are pure pass-through forwarders with no logic.
/// </summary>
internal static class ExportExtensionTestAccessors
{
    /// <summary>
    /// Test accessor for Export method.
    /// </summary>
    public static THttpResponseType ExportTac(this ExportExtension exportExtension, int zoneId, int appId, string name)
        => exportExtension.Export(zoneId: zoneId, appId: appId, name: name);
}
