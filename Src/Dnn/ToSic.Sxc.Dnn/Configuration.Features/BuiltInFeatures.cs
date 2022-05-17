using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Dnn.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static void Register(FeaturesCatalog cat) =>
            cat.Register(
                BuiltInFeatures.DnnPageWorkflowDisable
            );


    }
}
