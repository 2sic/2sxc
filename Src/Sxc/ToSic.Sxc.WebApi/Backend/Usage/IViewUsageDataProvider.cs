using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.Ui.Context.Sys.Dto;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Backend.Usage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IViewUsageDataProvider
{
    IEnumerable<ViewDto> Build(ICollection<IView> views, ICollection<BlockConfiguration> blocks, int siteId);
}
