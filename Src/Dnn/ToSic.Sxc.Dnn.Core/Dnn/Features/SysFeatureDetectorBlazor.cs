using ToSic.Eav.Internal.Features;
using ToSic.Sys.Capabilities.SysFeatures;
using static ToSic.Eav.Internal.Features.SysFeatureSuggestions;

namespace ToSic.Sxc.Dnn.Features;

internal class SysFeatureDetectorBlazor()
    : SysFeatureDetector(Blazor with { Name = Blazor.Name + " (not available in Dnn)" }, false);