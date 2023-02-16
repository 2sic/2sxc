using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public class BasicDynamicCodeRoot: DynamicCodeRoot<object, ServiceKit>
    {
        public BasicDynamicCodeRoot(MyServices services, WarnUseOfUnknown<BasicDynamicCodeRoot> _) : base(services, LogScopes.Base)
        {
        }
    }
}
