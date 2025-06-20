using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal class CodeTypedApiHelper(ExecutionContext exCtx) : CodeAnyApiHelper(exCtx), ICodeTypedApiHelper
{
    public IAppTyped AppTyped => ExCtx.AppTyped;
    public ITypedStack AllSettings => ExCtx.AllSettings;
    public ITypedStack AllResources => ExCtx.AllResources;
    public ServiceKit16 ServiceKit16 => ExCtx.GetKit<ServiceKit16>();
}