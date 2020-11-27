using ToSic.Eav.Context;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtPlatformContext: Platform
    {
        public OqtPlatformContext() => Type = PlatformType.Oqtane;
    }
}
