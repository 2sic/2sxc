using ToSic.Eav;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public class BasicDynamicCodeRoot: DynamicCodeRoot<object, ServiceKit>
    {
        public BasicDynamicCodeRoot(Dependencies dependencies, WarnUseOfUnknown<BasicDynamicCodeRoot> warn) : base(dependencies, LogNames.Basic)
        {
        }
    }
}
