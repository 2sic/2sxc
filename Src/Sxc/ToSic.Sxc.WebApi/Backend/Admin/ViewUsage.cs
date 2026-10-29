using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.Usage;
// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "View Usage",
    NameId = "0a095e44-7f00-4d36-8425-274c7ee7277a",
    NameIds = ["System.ViewUsage"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Blocks which use a view")]
public class ViewUsage : CustomDataSource
{
    [Configuration]
    public Guid ViewGuid => Configuration.GetThis(Guid.Empty);

    public ViewUsage(Dependencies services, LazySvc<UsageBackend> usage, IViewUsageDataProvider provider, IContextOfSiteService context)
        : base(services, "Sxc.ViewUsage", connect: [usage, provider, context])
        => ProvideOutRaw(() => Get(usage, provider, context.Context.Site));

    private IEnumerable<ViewUsageRaw> Get(LazySvc<UsageBackend> usage, IViewUsageDataProvider provider, ISite site)
        => usage.Value.ViewUsage(
                appId: AppId,
                guid: ViewGuid,
                finalBuilder: (views, blocks) => provider.Build(views, blocks, site.Id))
            .Select(view => new ViewUsageRaw(view));

    [ContentType(Name = "ViewUsage", Guid = "0a095e44-7f00-4d36-8425-274c7ee7277a", Scope = "System")]
    private sealed class ViewUsageRaw(ViewDto view) : IRawEntityAutoConvert
    {
        public int Id => view.Id;
        public Guid Guid => view.Guid;

        [ContentTypeTitle]
        public string Name => view.Name;

        public string? Path => view.Path;
        [ContentTypeField(Type = ValueTypes.Object)]
        public IEnumerable<ContentBlockDto> Blocks => view.Blocks;
    }
}