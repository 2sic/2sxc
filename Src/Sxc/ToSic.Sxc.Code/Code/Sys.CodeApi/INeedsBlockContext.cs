using ToSic.Sxc.Code.Context.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Interface for components that require access to the execution context.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface INeedsBlockContext
{
    /// <summary>
    /// Internal call to connect to the execution context.
    /// </summary>
    /// <param name="exCtx"></param>
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    void ConnectToRoot(ICodeContext exCtx);
}
