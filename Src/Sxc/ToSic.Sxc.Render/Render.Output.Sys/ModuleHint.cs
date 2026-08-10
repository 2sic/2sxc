using ToSic.Sys.Memory;

namespace ToSic.Sxc.Render.Output.Sys;

public class ModuleHint: ICanEstimateSize
{
    public string Message { get; init; }
    
    public SizeEstimate EstimateSize(ILog? log = default)
        => new(Message?.Length ?? 0);
}
