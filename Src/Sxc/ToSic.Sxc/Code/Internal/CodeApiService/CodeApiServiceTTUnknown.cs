using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Services;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Code.Internal;

internal class CodeApiServiceUnknown<TModel, TServiceKit>(CodeApiService.MyServices services, WarnUseOfUnknown<CodeApiServiceUnknown> _) : CodeApiService<object, ServiceKit>(services, LogScopes.Base)
{
}