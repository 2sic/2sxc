namespace ToSic.Sxc.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class Classification
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