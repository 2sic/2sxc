using System;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtPlatformContext: Platform
    {
        public override PlatformType Type => PlatformType.Oqtane;

        public override Version Version => new(Oqtane.Shared.Constants.Version);
    }
}
