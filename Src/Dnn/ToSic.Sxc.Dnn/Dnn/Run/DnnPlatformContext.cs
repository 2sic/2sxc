using ToSic.Eav.Context;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnPlatformContext: Platform
    {
        public DnnPlatformContext() => Type = PlatformType.Dnn;
    }
}
