namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface INeedsExecutionContext
{
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    void ConnectToRoot(IExecutionContext exCtx);
}