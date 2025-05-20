namespace ToSic.Sxc.Adam.Internal;

public static class AdamToLocalTypesExtensions
{
    //private static Exception GenerateException(object original)
    //    => new InvalidCastException($"Trying to cast {original?.GetType().Name} to a local type, but but this failed");

    public static File<TFolderId, TFileId> ToLocal<TFolderId, TFileId>(this IFile file)
        => (File<TFolderId, TFileId>)file;

    public static Folder<TFolderId, TFileId> ToLocal<TFolderId, TFileId>(this IFolder file)
        => (Folder<TFolderId, TFileId>)file;

}
