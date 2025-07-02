using ToSic.Sxc.Render.Sys;
using ToSic.Sys.Caching;
using ToSic.Sys.Memory;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class OutputCacheItem(IRenderResult data): ICanEstimateSize, ITimestamped
{
    public IRenderResult Data => data;

//#if NETFRAMEWORK
//    /// <summary>
//    /// This is only used in Dnn - might be solved with generics some time, but ATM this is just simpler
//    /// </summary>
//    public bool EnforcePre1025 = true;

//#endif
    public SizeEstimate EstimateSize(ILog? log = default) 
        => (data as ICanEstimateSize)?.EstimateSize(log)
           ?? new SizeEstimate(0, 0, Unknown: true);

    /// <summary>
    /// Timestamp info to better analyze cache data
    /// </summary>
    long ITimestamped.CacheTimestamp { get; } = DateTime.Now.Ticks;
}