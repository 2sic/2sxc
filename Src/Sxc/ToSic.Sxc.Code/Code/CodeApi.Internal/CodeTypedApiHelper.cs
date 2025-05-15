using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal class CodeTypedApiHelper: CodeAnyApiHelper, ICodeTypedApiHelper
{
    public IAppTyped AppTyped => Parent.AppTyped;
    public ITypedStack AllSettings => Parent.AllSettings;
    public ITypedStack AllResources => Parent.AllResources;
    public ServiceKit16 ServiceKit16 => Parent.GetKit<ServiceKit16>();
}