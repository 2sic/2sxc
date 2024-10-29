using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Images.Internal;
using static System.StringComparer;

namespace ToSic.Sxc.Images;

internal record TweakMedia(ResizeSettings Resize, ImageDecoratorVirtual VDec, ImageSpecs Img, PictureSpecs Pic, object ToolbarObj): ITweakMedia
{
    #region ResizeSettings

    public ITweakMedia Width(int width)
        => this with { Resize = Resize with { Width = width } };

    //public ITweakMedia Height(int height)
    //    => this with { Resize = Resize with { Height = height } };

    public ITweakMedia Factor(double factor)
        => this with { Resize = Resize with { Factor = factor } };

    #endregion

    #region Lightbox

    public ITweakMedia LightboxEnable(bool isEnabled = true)
        => this with { VDec = VDec with { LightboxIsEnabled = isEnabled } };

    public ITweakMedia LightboxGroup(string group)
        => this with { VDec = VDec with { LightboxGroup = group } };

    public ITweakMedia LightboxDescription(string description)
        => this with { VDec = VDec with { DescriptionExtended = description } };

    #endregion

    #region Image Properties

    public ITweakMedia ImgClass(string imgClass)
        => this with { Img = Img with { Class = imgClass } };

    public ITweakMedia ImgAlt(string imgAlt)
        => this with { Img = Img with { Alt = imgAlt } };

    public ITweakMedia ImgAltFallback(string imgAltFallback)
        => this with { Img = Img with { AltFallback = imgAltFallback } };

    public ITweakMedia ImgAttributes(IDictionary<string, string> attributes)
        => this with { Img = Img with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakMedia ImgAttributes(IDictionary<string, object> attributes)
        => this with { Img = Img with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakMedia ImgAttributes(object attributes)
        => this with { Img = Img with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    #endregion

    #region Picture Properties

    public ITweakMedia PictureClass(string pictureClass)
        => this with { Pic = Pic with { Class = pictureClass } };

    public ITweakMedia PictureAttributes(IDictionary<string, string> attributes)
        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };
    
    public ITweakMedia PictureAttributes(IDictionary<string, object> attributes)
        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakMedia PictureAttributes(object attributes)
        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    #endregion

    #region Toolbar
    
    public ITweakMedia Toolbar(bool enabled)
        => this with { ToolbarObj = enabled };

    public ITweakMedia Toolbar(IToolbarBuilder toolbar)
        => this with { ToolbarObj = toolbar };

    #endregion

    internal static IDictionary<string, object> CreateAttribDic(object attributes, string name)
        => attributes switch
        {
            null => null,
            IDictionary<string, object> ok => ok.ToInvariant(),
            IDictionary<string, string> strDic => strDic.ToDictionary(pair => pair.Key, pair => pair.Value as object, InvariantCultureIgnoreCase),
            _ => attributes.IsAnonymous()
                ? attributes.ToDicInvariantInsensitive()
                : throw new ArgumentException($@"format of {name} unknown: {name.GetType().Name}", nameof(attributes))
        };
}

internal record ImageDecoratorVirtual(
    bool? LightboxIsEnabled = default,
    string LightboxGroup = default,
    string DescriptionExtended = default
) : IImageDecorator;

internal record ImageSpecs(
    string Class = default,
    string Alt = default,
    string AltFallback = default,
    IDictionary<string, object> Attributes = default
);

internal record PictureSpecs(
    string Class = default,
    IDictionary<string, object> Attributes = default
);
