using System.Drawing;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Images;

/// <summary>
/// Tweak API for various media settings.
/// Specifically meant for images and pictures.
///
/// Some methods such as PictureClass will only have an effect if used on Picture(...) methods. 
/// </summary>
public interface ITweakMedia
{

    public ITweakMedia Width(int width);

    public ITweakMedia Factor(double factor);

    // Note: Recipe is missing

    ITweakMedia LightboxEnable(bool isEnabled = true);

    ITweakMedia LightboxGroup(string group);

    ITweakMedia LightboxDescription(string description);

    ITweakMedia ImgClass(string imgClass);

    ITweakMedia ImgAlt(string imgAlt);

    ITweakMedia ImgAltFallback(string imgAltFallback);

    ITweakMedia ImgAttributes(IDictionary<string, string> attributes);

    ITweakMedia ImgAttributes(IDictionary<string, object> attributes);

    ITweakMedia ImgAttributes(object attributes);

    ITweakMedia PictureClass(string pictureClass);

    ITweakMedia PictureAttributes(IDictionary<string, string> attributes);

    ITweakMedia PictureAttributes(IDictionary<string, object> attributes);

    ITweakMedia PictureAttributes(object attributes);

    ITweakMedia Toolbar(bool enabled);

    ITweakMedia Toolbar(IToolbarBuilder toolbar);
}
