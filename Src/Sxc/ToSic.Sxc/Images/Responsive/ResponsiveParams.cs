using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

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
    public IResizeSettings Settings { get; }
    public string ImgAlt { get; }
    public string ImgAltFallback { get; }
    public string ImgClass { get; }
    public IDictionary<string, object> ImgAttributes { get; }
    public IDictionary<string, object> PictureAttributes { get; }

    public string PictureClass { get; }

    public object Toolbar { get; }

    internal ResponsiveParams(
        object target,
#pragma warning disable IDE0060
        NoParamOrder protector = default,
#pragma warning restore IDE0060
        IResizeSettings settings = default,
        string imgAlt = default,
        string imgAltFallback = default,
        string imgClass = default,
        IDictionary<string, object> imgAttributes = default,
        string pictureClass = default,
        IDictionary<string, object> pictureAttributes = default,
        object toolbar = default
    )
    {
        Field = target as IField ?? (target as IFromField)?.Field;
        HasMetadataOrNull = target as IHasMetadata ?? Field;
        Link = target as IHasLink ?? new HasLink(target as string);
        Settings = settings;
        ImgAlt = imgAlt;
        ImgAltFallback = imgAltFallback;
        ImgClass = imgClass;
        ImgAttributes = imgAttributes;
        PictureClass = pictureClass;
        PictureAttributes = pictureAttributes;
        Toolbar = toolbar;
    }
}