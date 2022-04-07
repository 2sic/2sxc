using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Configuration.Features
{
    public partial class BuiltIn
    {
        public static void Register(FeaturesCatalog cat) =>
            cat.Register(
                RazorThrowPartial,
                RenderThrowPartialSystemAdmin
            );


    }
}
