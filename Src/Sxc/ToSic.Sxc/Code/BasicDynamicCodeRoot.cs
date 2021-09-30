using ToSic.Eav;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Code
{
    public class BasicDynamicCodeRoot: DynamicCodeRoot
    {
        public BasicDynamicCodeRoot(Dependencies dependencies, WarnUseOfUnknown<BasicDynamicCodeRoot> warn) : base(dependencies, LogNames.Basic)
        {
        }
    }
}
