using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnPlatformContext: PlatformContext
    {
        public DnnPlatformContext() => Type = PlatformTypes.Dnn;
    }
}
