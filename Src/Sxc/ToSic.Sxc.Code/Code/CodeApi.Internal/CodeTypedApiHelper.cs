using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal class CodeTypedApiHelper: CodeAnyApiHelper, ICodeTypedApiHelper
{
    public IAppTyped AppTyped => Parent.AppTyped;
    public ServiceKit16 ServiceKit16 => Parent.GetKit<ServiceKit16>();
}