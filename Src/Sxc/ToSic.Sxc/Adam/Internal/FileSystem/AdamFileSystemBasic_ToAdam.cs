using System.IO;
using ToSic.Eav.Helpers;

namespace ToSic.Sxc.Adam.Internal;

public partial class AdamFileSystemBasic
{

    private File<string, string> ToAdamFile(string path)
    {
        var physicalPath = _adamPaths.PhysicalPath(path);
        var f = new FileInfo(physicalPath);
        var directoryName = f.Directory.Name;

        // todo: unclear if we need both, but we need the url for the compare-if-same-path
        var relativePath = _adamPaths.RelativeFromAdam(path);
        var relativeUrl = relativePath.ForwardSlash();
        return new(AdamManager)
        {
            FullName = f.Name,
            Extension = f.Extension.TrimStart('.'),
            Size = Convert.ToInt32(f.Length),
            SysId = relativeUrl,
            Folder = directoryName,
            ParentSysId = relativeUrl.Replace(f.Name, ""),
            Path = relativePath,

            Created = f.CreationTime,
            Modified = f.LastWriteTime,
            Name = Path.GetFileNameWithoutExtension(f.Name),
            Url = _adamPaths.Url(relativeUrl),
            PhysicalPath = physicalPath,
        };
    }

    private Folder<string, string> ToAdamFolder(string path)
    {
        var physicalPath = _adamPaths.PhysicalPath(path);
        var f = new DirectoryInfo(physicalPath);

        var relativePath = _adamPaths.RelativeFromAdam(path);
        return new(AdamManager)
        {
            Path = relativePath,
            SysId = relativePath,
            ParentSysId = FindParentUrl(path),
            Name = f.Name,
            Created = f.CreationTime,
            Modified = f.LastWriteTime,

            Url = _adamPaths.Url(relativePath),
            PhysicalPath = physicalPath,
        };
    }

    private static string FindParentUrl(string path)
    {
        var cleanedPath = path.ForwardSlash().TrimEnd('/');
        var lastSlash = cleanedPath.LastIndexOf('/');
        return lastSlash == -1 ? "" : cleanedPath.Substring(0, lastSlash);
    }

}