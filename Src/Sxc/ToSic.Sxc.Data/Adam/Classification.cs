namespace ToSic.Sxc.Adam;

/// <summary>
/// Helper to classify a file by type.
/// </summary>
/// <remarks>
/// This has been part of 2sxc since forever, but only shown in docs since v21.02.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Classification
{
    public const string Folder = "folder";
    public const string File = "file";
    public const string Image = "image";
    public const string Document = "document";

    public static string TypeName(string ext)
    {
        // Make sure it's lower and doesn't have a leading .
        switch (ext.ToLowerInvariant().Trim('.'))
        {
            case "png":
            case "jpg":
            case "jpgx":
            case "jpeg":
            case "gif":
            case "svg":
            case "webp":
                return Image;
            case "doc":
            case "docx":
            case "txt":
            case "pdf":
            case "xls":
            case "xlsx":
                return Document;
            default:
                return File;
        }
    }

    public static bool IsImage(string ext) => TypeName(ext) == Image;
}