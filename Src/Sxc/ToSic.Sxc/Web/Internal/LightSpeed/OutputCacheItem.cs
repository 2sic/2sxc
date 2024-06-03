using ToSic.Eav.Apps.Internal.Insights;
using ToSic.Lib.Data;
using ToSic.Lib.Memory;
using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OutputCacheItem(IRenderResult data): ICanEstimateSize
{
    public IRenderResult Data => data;

#if NETFRAMEWORK
    /// <summary>
    /// This is only used in Dnn - might be solved with generics some time, but ATM this is just simpler
    /// </summary>
    public bool EnforcePre1025 = true;
#endif
    public SizeEstimate EstimateSize(ILog log = default) 
        => (data as ICanEstimateSize)?.EstimateSize(log)
           ?? new SizeEstimate(0, 0, Unknown: true);
}