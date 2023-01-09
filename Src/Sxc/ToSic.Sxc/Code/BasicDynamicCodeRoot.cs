using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public class BasicDynamicCodeRoot: DynamicCodeRoot<object, ServiceKit>
    {
        public BasicDynamicCodeRoot(Dependencies dependencies, WarnUseOfUnknown<BasicDynamicCodeRoot> warn) : base(dependencies, LogScopes.Base)
        {
        }
    }
}
