using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Images;

/// <summary>
/// Helper class to handle all kinds of parameters passed to a responsive tag
/// </summary>
[PrivateApi]
internal record ResponsiveSpecs(ResponsiveSpecsOfTarget Target, TweakMedia Tweaker);

/// <summary>
/// Initial specs of a specific target - typically a field,
/// extracted / prepared for responsive image processing.
/// </summary>
/// <param name="FieldOrNull">The field used for this responsive output - can be null!</param>
/// <param name="HasMdOrNull"></param>
/// <param name="ImgDecoratorOrNull">Image Decorator of the current image.</param>
/// <param name="HasLinkOrNull"></param>
/// <param name="FieldImgDecoratorOrNull"></param>
internal record ResponsiveSpecsOfTarget(
    IField FieldOrNull,
    IHasMetadata HasMdOrNull,
    ImageDecorator ImgDecoratorOrNull,
    IHasLink HasLinkOrNull,
    ImageDecorator FieldImgDecoratorOrNull
)
{
    internal string ResizeSettingsOrNull => ImgDecoratorOrNull?.ResizeSettings?.NullIfNoValue()
                                            ?? FieldImgDecoratorOrNull?.ResizeSettings?.NullIfNoValue();

    /// <summary>
    /// The only reliable object which knows about the url - should never be null
    /// </summary>
    public IHasLink Link => HasLinkOrNull;

    internal static ResponsiveSpecsOfTarget ExtractSpecs(object target)
    {
        // Handle null and already-typed scenarios
        switch (target)
        {
            case null: return new(null, null, null, null, null);
            case ResponsiveSpecsOfTarget already: return already;
        }

        // Figure out what field it's from - either because it's a hyperlink - or an image inside a WYSIWYG field
        var field = target as IField ?? (target as IFromField)?.Field;

        // The main Metadata provider is either the target itself, or the field holding it (can be null)
        var mdProvider = target as IHasMetadata ?? field;

        // The primary decorator - either of the field (e.g. Hyperlink-field with recommendations etc.)
        // or using the metadata-provider of the target
        var imgDecorator = (field as Field)?.ImageDecoratorOrNull ?? ImageDecorator.GetOrNull(mdProvider, []);

        // get the image decorator of the field of the content-type - e.g. the WYSIWYG field or Hyperlink field
        var inputImgDecorator = ImageDecorator.GetOrNull(field?.Parent.Type[field.Name], []);

        // figure out the link - either because it's already an IHasLink object, or it's possibly a string (or null-link)
        var link = target as IHasLink ?? new HasLink(target as string);

        return new(field, mdProvider, imgDecorator, link, inputImgDecorator);
    }
}
