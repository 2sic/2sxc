using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExCtxComplete : IExecutionContext,
    IExecutionContextWithApp,
    IExecutionContextWithBlock,
    IExecutionContextWithCmsContext
{
    ICodeDataFactory Cdf { get; }
}
