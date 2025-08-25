using ToSic.Sys.Caching;

namespace ToSic.Sxc.Blocks.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IDependentApp
{
    int AppId { get; }
    bool IsSitePrimaryApp { get; }

    bool IsEnabled { get; }
    ICollection<string> PathsToMonitor { get; }
    List<string> CacheKeys { get; }
}