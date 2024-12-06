using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{

    public static readonly Feature ImageServiceMultipleSizes = new(){
        NameId = "ImageServiceMultipleSizes",
        Guid = new("9dab12db-85e5-4fb8-a100-2f009bf32f72"),
        Name = "Image Service - Multiple Sizes",
        IsPublic = false,
        Ui = false,
        Description = "Enables the ImageService to provide multiple sizes on <code>srcset</code> for <code>img</code> or <code>source</code> tags on a <code>picture</code>",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    };

    public static readonly Feature ImageServiceUseFactors = new()
    {
        NameId = "ImageServiceUseFactors",
        Guid = new("7d2ce824-b249-466f-928b-21567f4fa5da"),
        Name = "Image Service - Optimize Sizes by Factor",
        IsPublic = false,
        Ui = false,
        Description = "Will change how the size of images is calculated to vary by factor. So a 1/2 image will not be 670px but 600 when using Bootstrap5. The exact values are configured in the settings.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    };


    public static readonly Feature ImageServiceSetSizes = new()
    {
        NameId = "ImageServiceSetSizes",
        Guid = new("31c2c0b6-87c2-4014-89ba-98543858bb8a"),
        Name = "Image Service - Set sizes-attribute on Image Tags",
        IsPublic = false,
        Ui = false,
        Description = "The browser can pre-load faster if the img-tag has additional information about the final sizes of the image. The exact configuration can be adjusted in the settings.",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    };


    public static readonly Feature ImageServiceMultiFormat = new()
    {
        NameId = "ImageServiceMultiFormat",
        Guid = new("4262df94-3877-4a5a-ac86-20b4f9b38e87"),
        Name = "Image Service - Multiple Formats",
        IsPublic = false,
        Ui = false,
        Description = "Enables the ImageService to also provide WebP as better alternatives to JPG and PNG",
        Security = FeaturesCatalogRules.Security0Improved,
        LicenseRules = Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    };
}