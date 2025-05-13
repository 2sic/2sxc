using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Context.Internal;

internal class PlatformUnknown: Platform, IIsUnknown
{
    public PlatformUnknown(WarnUseOfUnknown<PlatformUnknown> _) { }
    public override PlatformType Type => PlatformType.Unknown;

    public override Version Version => new(0, 0, 0);
}