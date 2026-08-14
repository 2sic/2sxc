using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Sxc.Backend.Usage;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Dnn.Backend.Context;
using ToSic.Sxc.Dnn.Pages;

namespace ToSic.Sxc.Dnn.Backend.Admin;

public class DnnViewUsageDataProvider() : ServiceBase("Dnn.ViewUse"), IViewUsageDataProvider
{
    public IEnumerable<ViewDto> Build(ICollection<IView> views, ICollection<BlockConfiguration> blocks, int siteId)
    {
        var modules = new DnnPages(Log).AllModulesWithContent(siteId);
        return views.Select(view => view.Init(blocks, modules));
    }
}
