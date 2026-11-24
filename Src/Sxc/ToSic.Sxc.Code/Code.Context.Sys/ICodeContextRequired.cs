namespace ToSic.Sxc.Code.Context.Sys;

/// <summary>
/// Interface for components that require access to the execution context.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeContextRequired
{
    /// <summary>
    /// Internal call to connect to the execution context.
    /// </summary>
    /// <param name="exCtx"></param>
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    void ConnectToRoot(ICodeContext exCtx);
}
