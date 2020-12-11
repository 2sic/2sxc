using ToSic.Eav.Run;

namespace ToSic.Sxc.Context
{
    public class PlatformUnknown: Platform, IIsUnknown
    {
        public override PlatformType Type => PlatformType.Unknown;
    }
}
