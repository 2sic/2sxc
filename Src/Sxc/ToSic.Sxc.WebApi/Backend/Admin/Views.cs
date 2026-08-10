using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.SysData;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.Blocks.Sys.Work;

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

    public ViewUsage(Dependencies services, GenWorkPlus<WorkBlocks> blocks)
        : base(services, "Sxc.ViewUsage", connect: [blocks])
        => ProvideOutRaw(() => Get(blocks), options: Options);

    private IEnumerable<IRawEntity> Get(GenWorkPlus<WorkBlocks> blocks)
        => SysDataRaw.Many(blocks.New(blocks.CtxSvc.ContextPlus(AppId)).AllWithView().Where(b => b.View?.Guid == ViewGuid));
    private static DataFactoryOptions Options() => new() { AutoId = true, TypeName = "ViewUsage", AllowUnknownValueTypes = true };
}
