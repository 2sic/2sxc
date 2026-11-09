using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal class CodeTypedApiHelper(ExecutionContext exCtx) : CodeAnyApiHelper(exCtx), ICodeTypedApiHelper
{
    public IAppTyped AppTyped => field ??= ExCtx.GetService<IAppTyped>(reuse: true);
    public ITypedStack AllSettings => ExCtx.AllSettings;
    public ITypedStack AllResources => ExCtx.AllResources;
    public ServiceKit16 ServiceKit16 => ExCtx.GetKit<ServiceKit16>();
}