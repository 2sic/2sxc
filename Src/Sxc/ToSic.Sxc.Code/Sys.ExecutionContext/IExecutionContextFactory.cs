using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExecutionContextFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <param name="parentClassOrNull"></param>
    /// <param name="blockOrNull"></param>
    /// <param name="parentLog"></param>
    /// <param name="compatibilityFallback"></param>
    /// <returns></returns>
    IExecutionContext New(object? parentClassOrNull, IBlock? blockOrNull, ILog parentLog, int compatibilityFallback);
}