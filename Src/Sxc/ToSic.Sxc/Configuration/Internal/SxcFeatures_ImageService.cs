using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Configuration.Internal;

public partial class SxcFeatures
{

    public static readonly Feature ImageServiceMultipleSizes = new(
        "ImageServiceMultipleSizes",
        new Guid("9dab12db-85e5-4fb8-a100-2f009bf32f72"),
        "Image Service - Multiple Sizes",
        false,
        false,
        "Enables the ImageService to provide multiple sizes on <code>srcset</code> for <code>img</code> or <code>source</code> tags on a <code>picture</code>",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    );

    public static readonly Feature ImageServiceUseFactors = new(
        "ImageServiceUseFactors",
        new Guid("7d2ce824-b249-466f-928b-21567f4fa5da"),
        "Image Service - Optimize Sizes by Factor",
        false,
        false,
        "Will change how the size of images is calculated to vary by factor. So a 1/2 image will not be 670px but 600 when using Bootstrap5. The exact values are configured in the settings.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    );


    public static readonly Feature ImageServiceSetSizes = new(
        "ImageServiceSetSizes",
        new Guid("31c2c0b6-87c2-4014-89ba-98543858bb8a"),
        "Image Service - Set sizes-attribute on Image Tags",
        false,
        false,
        "The browser can pre-load faster if the img-tag has additional information about the final sizes of the image. The exact configuration can be adjusted in the settings.",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    );


    public static readonly Feature ImageServiceMultiFormat = new(
        "ImageServiceMultiFormat",
        new Guid("4262df94-3877-4a5a-ac86-20b4f9b38e87"),
        "Image Service - Multiple Formats",
        false,
        false,
        "Enables the ImageService to also provide WebP as better alternatives to Jpg and Png",
        FeaturesCatalogRules.Security0Improved,
        Eav.Internal.Features.BuiltInFeatures.ForPatronsPerfectionist
    );
}