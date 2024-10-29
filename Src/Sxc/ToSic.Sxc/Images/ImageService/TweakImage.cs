using ToSic.Eav.Generics;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Images.Internal;
using static System.StringComparer;

namespace ToSic.Sxc.Images;

internal record TweakImage(ImageDecoratorVirtual VDec, ImageSpecs Img, PictureSpecs Pic, object ToolbarObj): ITweakImage
{
    public ITweakImage LightboxEnable(bool isEnabled = true)
        => this with { VDec = VDec with { LightboxIsEnabled = isEnabled } };

    public ITweakImage LightboxGroup(string group)
        => this with { VDec = VDec with { LightboxGroup = group } };

    public ITweakImage LightboxDescription(string description)
        => this with { VDec = VDec with { DescriptionExtended = description } };

    public ITweakImage ImgClass(string imgClass)
        => this with { Img = Img with { Class = imgClass } };

    public ITweakImage ImgAlt(string imgAlt)
        => this with { Img = Img with { Alt = imgAlt } };

    public ITweakImage ImgAltFallback(string imgAltFallback)
        => this with { Img = Img with { AltFallback = imgAltFallback } };

    public ITweakImage ImgAttributes(IDictionary<string, string> attributes)
        => this with { Img = Img with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakImage ImgAttributes(IDictionary<string, object> attributes)
        => this with { Img = Img with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakImage ImgAttributes(object attributes)
        => this with { Img = Img with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakImage PictureClass(string pictureClass)
        => this with { Pic = Pic with { Class = pictureClass } };

    public ITweakImage PictureAttributes(IDictionary<string, string> attributes)
        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };
    
    public ITweakImage PictureAttributes(IDictionary<string, object> attributes)
        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakImage PictureAttributes(object attributes)
        => this with { Pic = Pic with { Attributes = CreateAttribDic(attributes, nameof(attributes)) } };

    public ITweakImage Toolbar(bool enabled)
        => this with { ToolbarObj = enabled };

    public ITweakImage Toolbar(IToolbarBuilder toolbar)
        => this with { ToolbarObj = toolbar };

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

internal record ImageDecoratorVirtual(bool? LightboxIsEnabled = default, string LightboxGroup = default, string DescriptionExtended = default) : IImageDecorator;

internal record ImageSpecs(string Class = default, string Alt = default, string AltFallback = default, IDictionary<string, object> Attributes = default);

internal record PictureSpecs(string Class = default, IDictionary<string, object> Attributes = default);