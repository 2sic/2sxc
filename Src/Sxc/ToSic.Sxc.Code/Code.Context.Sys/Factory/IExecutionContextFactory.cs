

namespace ToSic.Sxc.Code.Context.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExecutionContextFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <returns></returns>
    ICodeContext New(ExecutionContextOptions options);
}
