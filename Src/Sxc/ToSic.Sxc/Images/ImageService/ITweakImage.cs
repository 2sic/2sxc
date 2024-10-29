using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Images;

public interface ITweakImage
{
    ITweakImage LightboxEnable(bool isEnabled = true);

    ITweakImage LightboxGroup(string group);

    ITweakImage LightboxDescription(string description);

    ITweakImage ImgClass(string imgClass);

    ITweakImage ImgAlt(string imgAlt);

    ITweakImage ImgAltFallback(string imgAltFallback);

    ITweakImage ImgAttributes(IDictionary<string, string> attributes);

    ITweakImage ImgAttributes(IDictionary<string, object> attributes);

    ITweakImage ImgAttributes(object attributes);

    ITweakImage PictureClass(string pictureClass);

    ITweakImage PictureAttributes(IDictionary<string, string> attributes);

    ITweakImage PictureAttributes(IDictionary<string, object> attributes);

    ITweakImage PictureAttributes(object attributes);

    ITweakImage Toolbar(bool enabled);

    ITweakImage Toolbar(IToolbarBuilder toolbar);
}

public interface ITweakPicture : ITweakImage
{
    ITweakPicture PictureClass(string pictureClass);

    ITweakPicture PictureAttributes(IDictionary<string, string> attributes);

    ITweakPicture PictureAttributes(IDictionary<string, object> attributes);

    ITweakPicture PictureAttributes(object attributes);
}