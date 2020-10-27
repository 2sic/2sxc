using ToSic.Sxc.Run;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnPlatformContext: Sxc.Run.Context.PlatformContext
    {
        public DnnPlatformContext() => Id = Platforms.Dnn;
    }
}
