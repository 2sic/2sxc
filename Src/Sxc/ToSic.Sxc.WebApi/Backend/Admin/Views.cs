using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.Views;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "Views",
    NameId = "708afea8-33d6-48c0-a629-31a052663bc1",
    NameIds = ["System.Views"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Views of the current app")]
public class Views : CustomDataSource
{
    public Views(Dependencies services, LazySvc<ViewsBackend> views)
        : base(services, "Sxc.Views", connect: [views])
        => ProvideOutRaw(() => views.Value.GetAll(AppId), options: () =>new DataFactoryOptions { AutoId = false });
}