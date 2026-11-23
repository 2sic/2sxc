using ToSic.Sxc.Data;
using ToSic.Sys.Caching.PiggyBack;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExecutionContextMissing(): IInjectExcCtx, IIsUnknown
{
    private Exception ExecutionContextNotYetSet => new("The execution context has not been initialized!");
    public TService GetService<TService>() where TService : class => throw ExecutionContextNotYetSet;

    public PiggyBack PiggyBack => throw ExecutionContextNotYetSet;

    public ILog? Log => throw ExecutionContextNotYetSet;

    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default)
        where TService : class
        => throw ExecutionContextNotYetSet;

    public TService GetServiceForData<TService>()
        where TService : class
        => throw ExecutionContextNotYetSet;

    public ITypedStack AllSettings => throw ExecutionContextNotYetSet;

    public ITypedStack AllResources => throw ExecutionContextNotYetSet;
}
