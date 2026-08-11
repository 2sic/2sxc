using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.SysData;
using ToSic.Sxc.Backend.Usage;
using ToSic.Sxc.Backend.Views;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(NiceName = "Views", NameId = "708afea8-33d6-48c0-a629-31a052663bc1", NameIds = ["System.Views"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Views of the current app")]
public class Views : CustomDataSource
{
    public Views(Dependencies services, LazySvc<ViewsBackend> views)
        : base(services, "Sxc.Views", connect: [views])
        => ProvideOutRaw(() => views.Value.GetAll(AppId), options: Options);
    private static DataFactoryOptions Options() => new() { AutoId = false, TypeName = "View", AllowUnknownValueTypes = true };
}

[PrivateApi]
[VisualQuery(NiceName = "View Usage", NameId = "0a095e44-7f00-4d36-8425-274c7ee7277a", NameIds = ["System.ViewUsage"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Blocks which use a view")]
public class ViewUsage : CustomDataSource
{
    [Configuration]
    public Guid ViewGuid => Configuration.GetThis(Guid.Empty);

    public ViewUsage(Dependencies services, LazySvc<UsageBackend> usage, IViewUsageDataProvider provider, ISxcCurrentContextService context)
        : base(services, "Sxc.ViewUsage", connect: [usage, provider, context])
        => ProvideOutRaw(() => Get(usage, provider, context), options: Options);

    private IEnumerable<IRawEntity> Get(LazySvc<UsageBackend> usage, IViewUsageDataProvider provider, ISxcCurrentContextService context)
        => SysDataRaw.Many(usage.Value.ViewUsage(
            AppId,
            ViewGuid,
            (views, blocks) => provider.Build(views, blocks, context.GetExistingAppOrSet(AppId).Site.Id)));

    private static DataFactoryOptions Options() => new() { AutoId = true, TypeName = "ViewUsage", AllowUnknownValueTypes = true };
}
