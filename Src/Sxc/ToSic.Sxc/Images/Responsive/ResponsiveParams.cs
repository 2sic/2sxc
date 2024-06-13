using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Images;

/// <summary>
/// Helper class to handle all kinds of parameters passed to a responsive tag
/// </summary>
[PrivateApi]
internal class ResponsiveParams
{
    /// <summary>
    /// The only reliable object which knows about the url - can never be null
    /// </summary>
    public IHasLink Link { get; }

    /// <summary>
    /// The field used for this responsive output - can be null!
    /// </summary>
    public IField Field { get; }
    public IHasMetadata HasMetadataOrNull { get; }
    public IResizeSettings Settings { get; init; }
    public string ImgAlt { get; init; }
    public string ImgAltFallback { get; init; }
    public string ImgClass { get; init; }
    public IDictionary<string, object> ImgAttributes { get; init; }
    public IDictionary<string, object> PictureAttributes { get; init; }

    public string PictureClass { get; init; }

    public object Toolbar { get; init; }

    public ImageDecorator ImageDecoratorOrNull => Field?.ImageDecoratorOrNull
                                                  ?? ImageDecorator.GetOrNull(HasMetadataOrNull, []);

    internal ResponsiveParams(object target)
    {
        Field = target as IField ?? (target as IFromField)?.Field;
        HasMetadataOrNull = target as IHasMetadata ?? Field;
        Link = target as IHasLink ?? new HasLink(target as string);
    }

    public ResponsiveParams((IField FieldOrNull, IHasMetadata HasMdOrNull, ImageDecorator ImageDecoratorOrNull, IHasLink HasLinkOrNull) inits)
    {
        Field = inits.FieldOrNull;
        HasMetadataOrNull = inits.HasMdOrNull;
        Link = inits.HasLinkOrNull;
        //ImageDecoratorOrNull = inits.ImageDecoratorOrNull;
    }

    internal static (IField FieldOrNull, IHasMetadata HasMdOrNull, ImageDecorator ImageDecoratorOrNull, IHasLink HasLinkOrNull) Prepare(object target)
    {
        var field = target as IField ?? (target as IFromField)?.Field;
        var link = target as IHasLink ?? new HasLink(target as string);
        var mdProvider = target as IHasMetadata ?? field;
        var imageDecoratorOrNull = field?.ImageDecoratorOrNull ?? ImageDecorator.GetOrNull(mdProvider, []);

        return (field, mdProvider, imageDecoratorOrNull, link);
    }
}