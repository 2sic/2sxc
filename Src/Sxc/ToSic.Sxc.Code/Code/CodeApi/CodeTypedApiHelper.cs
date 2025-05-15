using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Code.CodeApi;

internal class CodeTypedApiHelper: CodeAnyApiHelper, ICodeTypedApiService
{
    public IAppTyped AppTyped => Parent.AppTyped;
}