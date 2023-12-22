using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

internal class DynamicCodeRootUnknown<TModel, TServiceKit>: DynamicCodeRoot<object, ServiceKit>
{
    public DynamicCodeRootUnknown(MyServices services, WarnUseOfUnknown<DynamicCodeRootUnknown> _) : base(services, LogScopes.Base)
    {
    }
}