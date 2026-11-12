using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.App;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "App Extensions",
    NameId = "94687214-88ea-48f7-9153-186c5c885227",
    NameIds = ["System.AppExtensions"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Extensions installed in the current app")]
public class AppExtensions : CustomDataSource
{
    public AppExtensions(Dependencies services, LazySvc<ExtensionReaderBackend> reader)
        : base(services, "Sxc.AppExts", connect: [reader])
        => ProvideOutRaw(() => reader.Value.GetExtensions(AppId));
}