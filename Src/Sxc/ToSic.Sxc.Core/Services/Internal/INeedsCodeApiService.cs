using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface INeedsCodeApiService
{
    [ShowApiWhenReleased(ShowApiMode.Never)] 
    void ConnectToRoot(IExecutionContext exCtx);
}