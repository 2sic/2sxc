using ToSic.Eav.Internal.Features;

namespace ToSic.Sxc.Dnn.Features;

internal partial class DnnBuiltInFeatures
{
    public static void Register(FeaturesCatalog cat) =>
        cat.Register(
            DnnPageWorkflow
        );


}