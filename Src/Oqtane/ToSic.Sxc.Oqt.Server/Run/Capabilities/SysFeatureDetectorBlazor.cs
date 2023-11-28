using ToSic.Eav.Internal.Features;
using static ToSic.Eav.Internal.Features.SysFeatureSuggestions;

namespace ToSic.Sxc.Oqt.Server.Run.Capabilities;

internal class SysFeatureDetectorBlazor: SysFeatureDetector
{
    public SysFeatureDetectorBlazor() : base(Blazor, true)
    {
    }
}