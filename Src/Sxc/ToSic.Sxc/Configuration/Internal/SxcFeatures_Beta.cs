using System;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{
    public static readonly Feature RazorThrowPartial = new(
        "RazorThrowPartial",
        new Guid("d5a327c5-db0f-472b-93b2-94e66b15e16b"),
        "Razor: Handle Errors in sub-components for all users",
        false,
        false,
        "Will render most of the page and only error on a partial-render, instead of breaking the entire module. If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
        FeaturesCatalogRules.Security0Neutral,
        Eav.Internal.Features.BuiltInFeatures.ForCorePlusEnabled
    );

    public static readonly Feature RenderThrowPartialSystemAdmin = new(
        "RenderThrowPartialSystemAdmin",
        new Guid("5b0c9379-2fef-4f6e-9022-4d3c50e894e5"),
        "Razor: Handle Errors in sub-components for system admins only (host users)",
        false,
        false,
        "Will render most of the page and only error on a partial-render, instead of breaking the entire module. But only when the sys-admin is viewing the page. If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
        FeaturesCatalogRules.Security0Neutral,
        Eav.Internal.Features.BuiltInFeatures.ForCorePlusEnabled
    );
}