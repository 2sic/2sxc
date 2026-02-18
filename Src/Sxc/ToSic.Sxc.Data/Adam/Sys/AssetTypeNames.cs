namespace ToSic.Sxc.Adam.Sys;

/// <summary>
/// Helper to classify a file by type.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AssetTypeNames
{
    public static string GetTypeName(string ext) =>
        // Make sure it's lower and doesn't have a leading .
        ext.ToLowerInvariant().Trim('.') switch
        {
            "png" or "jpg" or "jpgx" or "jpeg" or "gif" or "svg" or "webp" => AssetTypes.Image,
            "doc" or "docx" or "txt" or "pdf" or "xls" or "xlsx" => AssetTypes.Document,
            _ => AssetTypes.File
        };

    public static bool IsImage(string ext) => GetTypeName(ext) == AssetTypes.Image;
}