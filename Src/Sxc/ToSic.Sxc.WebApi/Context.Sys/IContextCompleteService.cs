using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// Service to provide the complete context.
/// </summary>
public interface IContextCompleteService: IContextOfSiteService
{
    /// <summary>
    /// Retrieve the current app with reader etc. or null if not available.
    /// </summary>
    /// <remarks>
    /// Can be standalone (App only) or with a block (App + Block).
    /// </remarks>
    IContextOfApp? AppContextOrNull { get; }

    /// <summary>
    /// Retrieve the current block - or return null if block is not known.
    /// </summary>
    IBlock? BlockOrNull { get; }
}
