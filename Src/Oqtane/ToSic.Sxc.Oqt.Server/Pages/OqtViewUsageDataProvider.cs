using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Sxc.Backend.Usage;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Oqt.Server.Pages;

public class OqtViewUsageDataProvider(Pages pages) : IViewUsageDataProvider
{
    public IEnumerable<ViewDto> Build(ICollection<IView> views, ICollection<BlockConfiguration> blocks, int siteId)
    {
        var modules = pages.AllModulesWithContent(siteId);
        return views.Select(view => pages.ViewDtoBuilder(view, blocks, modules));
    }
}
