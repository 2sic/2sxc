using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Oqt.Server.Run.Capabilities
{
    internal class SysFeatureDetectorBlazor: SysFeatureDetector
    {
        public SysFeatureDetectorBlazor() : base(Blazor, true)
        {
        }
    }
}
