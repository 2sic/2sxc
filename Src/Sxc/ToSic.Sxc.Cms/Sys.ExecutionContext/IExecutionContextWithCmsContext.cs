using ToSic.Sxc.Context;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExecutionContextWithCmsContext
{
    public ICmsContext CmsContext { get; }
}