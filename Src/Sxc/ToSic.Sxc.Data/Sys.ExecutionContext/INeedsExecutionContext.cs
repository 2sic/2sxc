namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Interface for components that require access to the execution context.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface INeedsExecutionContext
{
    /// <summary>
    /// Internal call to connect to the execution context.
    /// </summary>
    /// <param name="exCtx"></param>
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    void ConnectToRoot(IExecutionContext exCtx);
}