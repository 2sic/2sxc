using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

internal class DynamicCodeRootUnknown(DynamicCodeRoot.MyServices services, WarnUseOfUnknown<DynamicCodeRootUnknown> _) : DynamicCodeRoot<object, ServiceKit>(services, LogScopes.Base)
{
}