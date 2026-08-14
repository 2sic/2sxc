using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Backend.Usage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IViewUsageDataProvider
{
    IEnumerable<ViewDto> Build(ICollection<IView> views, ICollection<BlockConfiguration> blocks, int siteId);
}
