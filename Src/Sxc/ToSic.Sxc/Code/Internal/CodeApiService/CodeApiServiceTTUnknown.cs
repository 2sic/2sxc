using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

internal class CodeApiServiceUnknown<TModel, TServiceKit>(CodeApiService.MyServices services, WarnUseOfUnknown<CodeApiServiceUnknown> _) : CodeApiService<object, ServiceKit>(services, LogScopes.Base)
{
}