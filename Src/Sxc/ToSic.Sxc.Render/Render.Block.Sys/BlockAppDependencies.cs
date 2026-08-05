using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Render.Block.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockAppDependencies : IDependentApp
{
    public required int AppId { get; init; }
    public required bool IsSitePrimaryApp { get; init; }

    public LightSpeedDecorator? LightSpeedDecorator { get; init; }

    public required bool IsEnabled { get; init; }

    public required ICollection<string> PathsToMonitor { get; init; }

    public required List<string> CacheKeys { get; init; }
}