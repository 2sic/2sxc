namespace ToSic.Sxc.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SpecialFiles
{
    /// <summary>
    /// Name of the web.config file which is copied to the 2sxc folder.
    /// Probably only used in DNN
    /// </summary>
    public static readonly string WebConfigFileName = "web.config";

    /// <summary>
    /// Name of the template web.config file which is copied to each 2sxc-folder
    /// </summary>
    internal static readonly string WebConfigTemplateFile = "WebConfigTemplate.config";
}