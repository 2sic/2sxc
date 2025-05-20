using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Paths.Internal;

namespace ToSic.Sxc.Adam.FileSystem.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamFileSystemHelpers(IAdamPaths adamPaths) : ServiceBase("Sxc.AdmFil")
{
    public string EnsurePhysicalPath(string path)
    {
        path = path.Backslash();
        return path.StartsWith("adam", StringComparison.CurrentCultureIgnoreCase)
            ? adamPaths.PhysicalPath(path)
            : path;
    }

    /// <summary>
    /// When uploading a new file, we must verify that the name isn't used. 
    /// If it is used, walk through numbers to make a new name which isn't used. 
    /// </summary>
    /// <param name="serverPath"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string FindUniqueFileName(string serverPath, string fileName)
    {
        var l = Log.Fn<string>($"{serverPath}, {fileName}");

        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName);
        for (var i = 1; i < AdamConstants.MaxSameFileRetries 
                        && File.Exists(Path.Combine(serverPath, Path.GetFileName(fileName))); i++)
            fileName = $"{name}-{i}{ext}";

        return l.ReturnAndLog(fileName);
    }



    public bool TryToRenameFile(string originalWithPath, string newName)
    {
        var l = Log.Fn<bool>($"{newName}");

        if (!File.Exists(originalWithPath))
            return l.ReturnFalse($"Can't rename because source file does not exist {originalWithPath}");

        AdamPathsBase.ThrowIfPathContainsDotDot(newName);
        var path = FindParentPath(originalWithPath);
        var newFilePath = Path.Combine(path, newName);
        if (File.Exists(newFilePath))
            return l.ReturnFalse($"Can't rename because file with new name exists {newFilePath}");

        File.Move(originalWithPath, newFilePath);
        return l.ReturnTrue($"File renamed");
    }


    private static string FindParentPath(string path)
    {
        var cleanedPath = path.Backslash().TrimEnd('\\');
        var lastSlash = cleanedPath.LastIndexOf('\\');
        return lastSlash == -1 ? "" : cleanedPath.Substring(0, lastSlash);
    }

}