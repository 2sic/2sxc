using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{
    public static readonly Feature RazorThrowPartial = new()
    {
        NameId = "RazorThrowPartial",
        Guid = new("d5a327c5-db0f-472b-93b2-94e66b15e16b"),
        Name = "Razor= Handle Errors in sub-components for all users",
        IsPublic = false,
        Ui = false,
        Description = "Will render most of the page and only error on a partial-render, instead of breaking the entire module. If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = BuiltInFeatures.ForCorePlusEnabled
    };

    public static readonly Feature RenderThrowPartialSystemAdmin = new()
    {
        NameId = "RenderThrowPartialSystemAdmin",
        Guid = new("5b0c9379-2fef-4f6e-9022-4d3c50e894e5"),
        Name = "Razor= Handle Errors in sub-components for system admins only (host users)",
        IsPublic = false,
        Ui = false,
        Description = "Will render most of the page and only error on a partial-render, instead of breaking the entire module. But only when the sys-admin is viewing the page. If enabled, then Html.Render or similar activities which throw an error won't stop the entire module, but just that part. ",
        Security = FeaturesCatalogRules.Security0Neutral,
        LicenseRules = BuiltInFeatures.ForCorePlusEnabled
    };

    public static readonly Feature PermissionPrioritizeModuleContext = new()
    {
        NameId = nameof(PermissionPrioritizeModuleContext),
        Guid = new("3533a6e7-9cd9-4f2e-8978-f426a1a2694f"),
        Name = "Give restricted editors more permissions when editing inner content. ",
        IsPublic = false,
        Ui = false,
        Description = "This modifies the permission system to give the user permissions granted by the page or module, even if they switched to another App.",
        Security = new(2, "Reduces security. Restricted editors are able to access other Apps which doesn't make sense for restricted editors."),
        LicenseRules = BuiltInFeatures.ForCorePlusDisabled
    };

}