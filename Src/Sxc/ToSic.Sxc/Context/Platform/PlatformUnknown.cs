using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Context
{
    public class PlatformUnknown: Platform, IIsUnknown
    {
        public PlatformUnknown(WarnUseOfUnknown<PlatformUnknown> warn) { }
        public override PlatformType Type => PlatformType.Unknown;
    }
}
