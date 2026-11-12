using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.App;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "App Extension Details",
    NameId = "641ec34d-4e04-4682-812a-0dda1e65f905",
    NameIds = ["System.AppExtensionDetails", "System.AppExtensionInspect"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Details, files and content types of an app extension")]
public class AppExtensionDetails : CustomDataSource
{
    private ExtensionInspectResultDto? _result;
    [Configuration(Fallback = "")]
    public string ExtensionName => Configuration.GetThis<string>("");

    [Configuration(Fallback = "")]
    public string Edition => Configuration.GetThis<string>("");

    public AppExtensionDetails(Dependencies services, LazySvc<ExtensionInspectBackend> inspect)
        : base(services, "Sxc.ExtDetails", connect: [inspect])
    {
        ProvideOutRaw(() => Details(inspect), options: () => new() { TypeName = "AppExtensionDetails" });
        // Compatibility for deployed UIs which still use System.AppExtensionInspect.
        ProvideOutRaw(() => Details(inspect), name: "State", options: () => new() { TypeName = "AppExtensionDetails" });
        ProvideOutRaw(() => Files(inspect), name: "Files", options: () => new() { TypeName = "AppExtensionFile" });
        ProvideOutRaw(() => Summary(inspect), name: "Summary", options: () => new() { TypeName = "AppExtensionSummary" });
        ProvideOutRaw(() => ContentTypes(inspect), name: "ContentTypes", options: () => new() { TypeName = "AppExtensionContentType" });
    }

    private ExtensionInspectResultDto Result(LazySvc<ExtensionInspectBackend> inspect) => _result ??= inspect.Value.Inspect(AppId, ExtensionName, Edition);
    private IEnumerable<ExtensionDetailsRaw> Details(LazySvc<ExtensionInspectBackend> inspect)
        => [new(Result(inspect).FoundLock)];
    private IEnumerable<ExtensionFileStatusDto> Files(LazySvc<ExtensionInspectBackend> inspect)
        => Result(inspect).Files ?? [];
    private IEnumerable<ExtensionInspectSummaryDto> Summary(LazySvc<ExtensionInspectBackend> inspect)
        => Result(inspect).Summary is { } summary ? [summary] : [];
    private IEnumerable<ExtensionInspectContentTypeDto> ContentTypes(LazySvc<ExtensionInspectBackend> inspect)
        => Result(inspect).ContentTypes;
    private sealed record ExtensionDetailsRaw(bool FoundLock) : IRawEntityAutoConvert;
}