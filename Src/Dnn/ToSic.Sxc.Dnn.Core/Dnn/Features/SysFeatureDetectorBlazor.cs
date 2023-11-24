using ToSic.Eav.Internal.Features;
using static ToSic.Eav.Internal.Features.SysFeatureSuggestions;

namespace ToSic.Sxc.Dnn.Features
{
    internal class SysFeatureDetectorBlazor: SysFeatureDetector
    {
        public SysFeatureDetectorBlazor() : base(Blazor.Clone(name: Blazor.Name + " (not available in Dnn)"), false)
        {
        }
    }
}
