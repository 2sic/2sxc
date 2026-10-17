namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExecutionContextFactory
{
    /// <summary>
    /// Creates a CodeApiService - if possible based on the parent class requesting it.
    /// </summary>
    /// <returns></returns>
    IExecutionContext New(ExecutionContextOptions options);
}