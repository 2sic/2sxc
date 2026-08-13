using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.App;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(NiceName = "App Extensions", NameId = "94687214-88ea-48f7-9153-186c5c885227", NameIds = ["System.AppExtensions"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Extensions installed in the current app")]
public class AppExtensions : CustomDataSource
{
    public AppExtensions(Dependencies services, LazySvc<ExtensionReaderBackend> reader)
        : base(services, "Sxc.AppExts", connect: [reader])
        => ProvideOutRaw(() => reader.Value.GetExtensions(AppId).Extensions, options: Options);

    private static DataFactoryOptions Options() => new() { TypeName = "AppExtension", AllowUnknownValueTypes = true };
}

[PrivateApi]
[VisualQuery(NiceName = "App Extension Inspect", NameId = "641ec34d-4e04-4682-812a-0dda1e65f905", NameIds = ["System.AppExtensionInspect"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Inspect an app extension and its files")]
public class AppExtensionInspect : CustomDataSource
{
    private ExtensionInspectResultDto? _result;
    [Configuration(Field = "Name", Fallback = "")]
    public string ExtensionName => Configuration.GetThis<string>("");

    [Configuration(Fallback = "")]
    public string Edition => Configuration.GetThis<string>("");

    public AppExtensionInspect(Dependencies services, LazySvc<ExtensionInspectBackend> inspect)
        : base(services, "Sxc.ExtInspect", connect: [inspect])
    {
        ProvideOutRaw(() => State(inspect), name: "State", options: Options);
        ProvideOutRaw(() => Files(inspect), name: "Files", options: Options);
        ProvideOutRaw(() => Summary(inspect), name: "Summary", options: Options);
        ProvideOutRaw(() => ContentTypes(inspect), name: "ContentTypes", options: Options);
    }

    private ExtensionInspectResultDto Result(LazySvc<ExtensionInspectBackend> inspect) => _result ??= inspect.Value.Inspect(AppId, ExtensionName, Edition);
    private IEnumerable<ExtensionInspectStateRaw> State(LazySvc<ExtensionInspectBackend> inspect)
        => [new(Result(inspect).FoundLock)];
    private IEnumerable<ExtensionFileStatusDto> Files(LazySvc<ExtensionInspectBackend> inspect)
        => Result(inspect).Files ?? [];
    private IEnumerable<ExtensionInspectSummaryDto> Summary(LazySvc<ExtensionInspectBackend> inspect)
        => Result(inspect).Summary is { } summary ? [summary] : [];
    private IEnumerable<ExtensionInspectContentTypeDto> ContentTypes(LazySvc<ExtensionInspectBackend> inspect)
        => Result(inspect).Data?.ContentTypes ?? [];
    private static DataFactoryOptions Options() => new() { AllowUnknownValueTypes = true };

    private sealed record ExtensionInspectStateRaw(bool FoundLock) : IRawEntityAutoConvert;
}
