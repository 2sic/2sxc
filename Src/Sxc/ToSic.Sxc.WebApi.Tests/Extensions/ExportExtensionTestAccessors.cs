using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Apps.Extensions.Sys.Export;
using ToSic.Sxc.Backend.App;
using FileResult = ToSic.Eav.ImportExport.Sys.FileResult;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Test accessor methods for ExtensionExportService to isolate test usage from production code references.
/// These are pure pass-through forwarders with no logic.
/// </summary>
internal static class ExportExtensionTestAccessors
{
    /// <summary>
    /// Test accessor for Export method.
    /// </summary>
    public static FileResult ExportTac(this ExtensionExportService exportExtension, int zoneId, int appId, string name)
        => exportExtension.Export(appId: appId, name: name);
}
