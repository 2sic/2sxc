using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

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
        string method,
        object target,
        string noParamOrder = Parameters.Protector,
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
        Parameters.ProtectAgainstMissingParameterNames(noParamOrder, method,
            $"{nameof(target)}, {nameof(settings)}, factor, {nameof(imgAlt)}, {nameof(imgClass)}, recipe");

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