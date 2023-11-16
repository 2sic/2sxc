using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    internal class SysFeatureDetectorBlazor: SysFeatureDetector
    {
        public SysFeatureDetectorBlazor() : base(Blazor.Clone(name: Blazor.Name + " (not available in Dnn)"), false)
        {
        }
    }
}
