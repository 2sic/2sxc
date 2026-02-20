using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExecutionContextFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <returns></returns>
    IExecutionContext New(ExecutionContextOptions options);
}

public record ExecutionContextOptions
{
    public object? OwnerOrNull { get; init; }
    public IBlock? BlockOrNull { get; init; }
    public required ILog ParentLog { get; init; }
    public int CompatibilityFallback { get; init; }
}