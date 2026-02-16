namespace ToSic.Sxc.Adam;

/// <summary>
/// Provides constant string values that represent common ADAM (Asset Digital Asset Management) item types.
/// </summary>
/// <remarks>
/// The constants can be used to identify or compare asset types such as
/// folders, files, images, and documents within ADAM-related APIs.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class AssetTypes
{
    /// <summary>
    /// The asset is a folder.
    /// </summary>
    public const string Folder = "folder";

    /// <summary>
    /// The asset is a file, specifically one which is not detected to be an image or a document.
    /// </summary>
    public const string File = "file";

    /// <summary>
    /// The asset is an image.
    /// </summary>
    public const string Image = "image";

    /// <summary>
    /// The asset is a document.
    /// </summary>
    public const string Document = "document";
}
