using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Backend.InPage;

/// <summary>
/// Options used by the quick dialog when configuring a block.
/// </summary>
[PrivateApi]
[VisualQuery(
    NiceName = "Block Configuration Options",
    NameId = "a90e4af9-7f9f-4690-bb4b-98f3ac4d7a17",
    NameIds = ["System.BlockConfigOptions"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Apps, content types and compatible views for configuring a block")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockConfigOptions : CustomDataSource
{
    [Configuration(Fallback = "")]
    public string Apps => Configuration.GetThis(fallback: "");

    public BlockConfigOptions(
        Dependencies services,
        LazySvc<AppViewPickerBackend> viewPicker,
        LazySvc<WorkApps> workApps,
        LazySvc<IContextOfSiteService> siteContext)
        : base(services, "Sxc.BlkCfgOpt", connect: [viewPicker, workApps, siteContext])
    {
        ProvideOutRaw(
            name: "Templates",
            data: () => viewPicker.Value.Templates());
        ProvideOutRaw(
            name: "ContentTypes",
            data: () => viewPicker.Value.ContentTypes());
        ProvideOutRaw(
            name: "Apps",
            data: () => workApps.Value.GetSelectableApps(siteContext.Value.Context.Site, Apps));
    }
}
