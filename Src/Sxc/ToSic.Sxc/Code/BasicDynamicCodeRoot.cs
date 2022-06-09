using ToSic.Eav;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Services.Kits;

namespace ToSic.Sxc.Code
{
    public class BasicDynamicCodeRoot: DynamicCodeRoot<object, KitNone>
    {
        public BasicDynamicCodeRoot(Dependencies dependencies, WarnUseOfUnknown<BasicDynamicCodeRoot> warn) : base(dependencies, LogNames.Basic)
        {
        }
    }
}
