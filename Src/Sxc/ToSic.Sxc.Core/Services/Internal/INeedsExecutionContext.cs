using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface INeedsExecutionContext
{
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    void ConnectToRoot(IExecutionContext exCtx);
}