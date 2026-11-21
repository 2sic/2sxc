using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// Service to provide the complete context.
/// </summary>
public interface IContextCompleteService: IContextOfAppService
{

    /// <summary>
    /// Retrieve the current block - or return null if block is not known.
    /// </summary>
    IBlock? BlockOrNull { get; }
}
