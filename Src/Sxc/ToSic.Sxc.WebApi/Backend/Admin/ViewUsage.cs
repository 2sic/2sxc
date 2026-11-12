using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Ui.Context.Sys.Dto;
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
public class ViewUsage(
    CustomDataSource.Dependencies services,
    LazySvc<UsageBackend> usage,
    IViewUsageDataProvider provider,
    IContextOfSiteService context)
    : CustomDataSource(services, "Sxc.ViewUsage", connect: [usage, provider, context])
{
    [Configuration]
    public Guid ViewGuid => Configuration.GetThis(Guid.Empty);

    protected override IEnumerable<IRawData> GetDefault()
    {
        var viewInfo = usage.Value.ViewUsage(appId: AppId, guid: ViewGuid);
        var viewDtoList = provider.Build(viewInfo.Views, viewInfo.BlockConfigs, context.Context.Site.Id);
        return viewDtoList
            .Select(vDto => new ViewUsageRaw(vDto));
    }

    // ReSharper disable UnusedMember.Local
    [ContentType(
        Name = "ViewUsage",
        Guid = "0a095e44-7f00-4d36-8425-274c7ee7277a",
        Scope = "System")]
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
    // ReSharper restore UnusedMember.Local
}