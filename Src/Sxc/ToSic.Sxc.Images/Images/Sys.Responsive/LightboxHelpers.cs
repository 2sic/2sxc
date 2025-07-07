using ToSic.Sys.Utils;

namespace ToSic.Sxc.Images;

internal class LightboxHelpers
{
    internal const string Attribute = "data-cms-lightbox";
    internal const string AttributeGroup = "data-cms-lightbox-group";
    internal const string SettingsName = "Lightbox";
    internal const string JsCall = "window.Fancybox.bind()";

    /// <summary>
    /// Create the args-list for the lightbox initialization
    /// </summary>
    /// <param name="imageGroup">Name of the image group</param>
    /// <returns></returns>
    internal static object[] CreateArgs(string? imageGroup)
    {
        var hasGroup = imageGroup.HasValue();
        return
        [
            hasGroup
                ? $"[{AttributeGroup}=\"{imageGroup}\"]"
                : $"[{Attribute}=\"\"]",
            new
            {
                groupAll = hasGroup,
                Thumbs = new
                {
                    autoStart = false
                }
            }
        ];
    }
}