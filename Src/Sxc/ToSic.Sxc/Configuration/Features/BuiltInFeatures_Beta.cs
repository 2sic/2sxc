using System;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Features
{
    public partial class BuiltInFeatures
    {
        public static readonly Feature RazorThrowPartial = new Feature(
            "RazorThrowPartial",
            new Guid("d5a327c5-db0f-472b-93b2-94e66b15e16b"),
            "Will render most of the page and only error on a partial-render, instead of breaking the entire module. ",
            false,
            false,
            "If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Internal.Features.BuiltInFeatures.ForCorePlusEnabled
        );

        public static readonly Feature RenderThrowPartialSystemAdmin = new Feature(
            "RenderThrowPartialSystemAdmin",
            new Guid("5b0c9379-2fef-4f6e-9022-4d3c50e894e5"),
            "Will render most of the page and only error on a partial-render, instead of breaking the entire module. But only when the sys-admin is viewing the page.",
            false,
            false,
            "If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
            FeaturesCatalogRules.Security0Neutral,
            Eav.Internal.Features.BuiltInFeatures.ForCorePlusEnabled
        );
    }
}
