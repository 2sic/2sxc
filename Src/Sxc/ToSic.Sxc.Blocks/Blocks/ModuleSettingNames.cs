namespace ToSic.Sxc.Blocks;

/// <summary>
/// Contains special constants for setting-names stored in the Dnn/Oqtane module settings.
/// </summary>
/// <remarks>
/// Note that for historical reasons, the keys where different in Dnn and Oqtane until 2025-08-19.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleSettingNames
{
    // Important note: always use static-readonly, NOT constant
    // This prevents the value from being compiled into other DLLs,
    // So if a value ever changes, it will always be retrieved from here


    /// <summary>
    /// This setting will store what App is to be shown on a module. 
    /// The value must contain the Guid/Name (so the word "Default" or the app guid)
    /// </summary>
    /// <remarks>
    /// "EavApp" in Oqtane until 2025-08-19
    /// </remarks>
    public static readonly string AppName = "TsDynDataApp";

    /// <summary>
    /// This key is for storing the setting, which content-group (bundle/block) is to be shown in the module.
    /// The value will be a GUID. 
    /// </summary>
    /// <remarks>
    /// "EavContentGroup" in Oqtane until 2025-08-19
    /// </remarks>
    public static readonly string ContentGroup = "TsDynDataContentGroup";

    /// <summary>
    /// This is used to store the Guid of the Preview-View in the module settings.
    /// The preview is only used till the App has a real content-group attached,
    /// after which the content-group will provide the correct view. 
    /// </summary>
    /// <remarks>
    /// "EavPreview" in Oqtane until 2025-08-19
    /// </remarks>
    public static readonly string PreviewView = "TsDynDataPreview";

}