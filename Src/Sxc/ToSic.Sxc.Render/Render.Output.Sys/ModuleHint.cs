using ToSic.Sys.Memory;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Render.Output.Sys;

public class ModuleHint: ICanEstimateSize
{
    public required string Message { get; init; }
    
    public UserElevation ForUserElevation { get; init; } = UserElevation.SystemAdmin;

    public SizeEstimate EstimateSize(ILog? log = default)
        => new(Message?.Length ?? 0);
}
