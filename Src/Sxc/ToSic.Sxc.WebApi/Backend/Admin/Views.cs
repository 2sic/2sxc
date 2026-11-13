using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Raw;
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
public class Views(CustomDataSource.Dependencies services, AppWorkQuick<WorkViewsGetDetailed> views)
    : CustomDataSource(services, "Sxc.Views", connect: [views])
{
    protected override IEnumerable<IRawData> GetDefault()
        => views.New(this.PureIdentity()).GetAll(AppId);
}