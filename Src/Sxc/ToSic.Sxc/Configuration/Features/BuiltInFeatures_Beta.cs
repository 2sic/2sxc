using System;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static readonly FeatureDefinition RazorThrowPartial = new FeatureDefinition(
            "RazorThrowPartial",
            new Guid("d5a327c5-db0f-472b-93b2-94e66b15e16b"),
            "Will render most of the page and only error on a partial-render, instead of breaking the entire module. ",
            false,
            false,
            "If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Configuration.BuiltInFeatures.ForBeta
        );

        public static readonly FeatureDefinition RenderThrowPartialSystemAdmin = new FeatureDefinition(
            "RenderThrowPartialSystemAdmin",
            new Guid("5b0c9379-2fef-4f6e-9022-4d3c50e894e5"),
            "Will render most of the page and only error on a partial-render, instead of breaking the entire module. But only when the sys-admin is viewing the page.",
            false,
            false,
            "If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Configuration.BuiltInFeatures.ForBetaEnabled
        );
    }
}
