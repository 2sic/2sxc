using System;
using ToSic.Eav.Internal.Unknown;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Context
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class PlatformUnknown: Platform, IIsUnknown
    {
        public PlatformUnknown(WarnUseOfUnknown<PlatformUnknown> _) { }
        public override PlatformType Type => PlatformType.Unknown;

        public override Version Version => new Version(0, 0, 0);
    }
}
