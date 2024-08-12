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
internal class ResponsiveParams(ResponsiveParams.PreparedResponsiveParams prepared)
{
    /// <summary>
    /// The only reliable object which knows about the url - can never be null
    /// </summary>
    public IHasLink Link => prepared.HasLinkOrNull;

    /// <summary>
    /// The field used for this responsive output - can be null!
    /// </summary>
    public IField Field => prepared.FieldOrNull;

    public IHasMetadata HasMetadataOrNull => prepared.HasMdOrNull;

    public ImageDecorator ImageDecoratorOrNull => prepared.ImgDecoratorOrNull;

    public ImageDecorator InputImageDecoratorOrNull => prepared.InputImgDecoratorOrNull;

    public IResizeSettings Settings { get; init; }
    public string ImgAlt { get; init; }
    public string ImgAltFallback { get; init; }
    public string ImgClass { get; init; }
    public IDictionary<string, object> ImgAttributes { get; init; }
    public IDictionary<string, object> PictureAttributes { get; init; }

    public string PictureClass { get; init; }

    public object Toolbar { get; init; }

    internal static PreparedResponsiveParams Prepare(object target)
    {
        // Handle null and already-typed scenarios
        switch (target)
        {
            case null: return new(null, null, null, null, null);
            case PreparedResponsiveParams already: return already;
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

    internal record PreparedResponsiveParams(
        IField FieldOrNull,
        IHasMetadata HasMdOrNull,
        ImageDecorator ImgDecoratorOrNull,
        IHasLink HasLinkOrNull,
        ImageDecorator InputImgDecoratorOrNull
    )
    {
        internal string ResizeSettingsOrNull => ImgDecoratorOrNull?.ResizeSettings?.NullIfNoValue()
                                               ?? InputImgDecoratorOrNull?.ResizeSettings?.NullIfNoValue();
    }
}