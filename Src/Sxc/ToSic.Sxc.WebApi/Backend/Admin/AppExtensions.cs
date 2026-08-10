using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Backend.SysData;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(NiceName = "App Extensions", NameId = "94687214-88ea-48f7-9153-186c5c885227", NameIds = ["System.AppExtensions"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Extensions installed in the current app")]
public class AppExtensions : CustomDataSource
{
    public AppExtensions(Dependencies services, LazySvc<ExtensionReaderBackend> reader)
        : base(services, "Sxc.AppExts", connect: [reader])
        => ProvideOutRaw(() => SysDataRaw.Many(reader.Value.GetExtensions(AppId).Extensions), options: Options);

    private static DataFactoryOptions Options() => new() { AutoId = true, TitleField = "Folder", TypeName = "AppExtension", AllowUnknownValueTypes = true };
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
    private IEnumerable<IRawEntity> State(LazySvc<ExtensionInspectBackend> inspect) => [SysDataRaw.One(new { Result(inspect).FoundLock })];
    private IEnumerable<IRawEntity> Files(LazySvc<ExtensionInspectBackend> inspect) => SysDataRaw.Many(Result(inspect).Files);
    private IEnumerable<IRawEntity> Summary(LazySvc<ExtensionInspectBackend> inspect) => Result(inspect).Summary is { } x ? [SysDataRaw.One(x)] : [];
    private IEnumerable<IRawEntity> ContentTypes(LazySvc<ExtensionInspectBackend> inspect) => SysDataRaw.Many(Result(inspect).Data?.ContentTypes);
    private static DataFactoryOptions Options() => new() { AutoId = true, AllowUnknownValueTypes = true };
}
