using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Options necessary to initialize a fresh execution context.
/// </summary>
public record ExecutionContextOptions
{
    /// <summary>
    /// The owning object - usually a razor file or a WebApi - providing version information for compatibility
    /// </summary>
    public object? OwnerOrNull { get; init; }

    /// <summary>
    /// The block for this execution context, if available.
    /// </summary>
    public IBlock? BlockOrNull { get; init; }

    /// <summary>
    /// The module we're on, in case the block is not known.
    /// </summary>
    public IModule? ModuleIfBlockUnknown { get; init; }

    /// <summary>
    /// The parent log to attach to. Required.
    /// </summary>
    public required ILog ParentLog { get; init; }

    /// <summary>
    /// Fallback compatibility to use if the Owner is unknown/null or if it does not implement <see cref="ICompatibilityLevel"/>.
    /// </summary>
    public int CompatibilityFallback { get; init; }

    /// <summary>
    /// The true compatibility to apply.
    /// </summary>
    public int Compatibility => _compatibility
        ??= (OwnerOrNull as ICompatibilityLevel)?.CompatibilityLevel ?? CompatibilityFallback;

    private int? _compatibility;
}