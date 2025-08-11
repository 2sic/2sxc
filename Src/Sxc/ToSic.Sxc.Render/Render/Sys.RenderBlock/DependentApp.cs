using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Render.Sys.RenderBlock;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DependentApp : IDependentApp
{
    public required int AppId { get; init; }
    public required bool IsSitePrimaryApp { get; init; }

    public LightSpeedDecorator? LightSpeedDecorator { get; init; }

    public required bool IsEnabled { get; init; }

    public required ICollection<string> PathsToMonitor { get; init; }

    public required List<string> CacheKeys { get; init; }
}