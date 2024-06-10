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
    public IResizeSettings Settings { get; init; }
    public string ImgAlt { get; init; }
    public string ImgAltFallback { get; init; }
    public string ImgClass { get; init; }
    public IDictionary<string, object> ImgAttributes { get; init; }
    public IDictionary<string, object> PictureAttributes { get; init; }

    public string PictureClass { get; init; }

    public object Toolbar { get; init; }

    internal ResponsiveParams(object target)
    {
        Field = target as IField ?? (target as IFromField)?.Field;
        HasMetadataOrNull = target as IHasMetadata ?? Field;
        Link = target as IHasLink ?? new HasLink(target as string);
    }
}