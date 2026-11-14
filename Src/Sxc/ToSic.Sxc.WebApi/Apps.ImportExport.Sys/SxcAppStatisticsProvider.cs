using ToSic.Eav.Apps.Inspect.Sys;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.DataProviders;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Apps.ImportExport.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcAppStatisticsProvider(AppWorkChain<AppStatisticsProvider> data, AppWorkChain<WorkViewsGet> workViews)
    : ServiceWithSetup<IAppWorkContext>("Bck.Export", connect: [data, workViews]),
        IRawDataProviderWithCtx
{

    public IEnumerable<IRawData> Provide(IDataSourceConfiguration configuration)
    {
        // Get original data
        var stats = data.New(MyOptions).GetAppInfo();

        // Augment it with views information, which is a concept not available in EAV
        var appViews = workViews.New(MyOptions);
        var allViews = appViews.AllViews;
        stats = stats with
        {
            TemplatesCount = allViews.Count,
            HasRazorTemplates = allViews.Any(t => t.IsRazor()),
            HasTokenTemplates = allViews.Any(t => !t.IsRazor()),

        };
        return [stats];
    }
}
