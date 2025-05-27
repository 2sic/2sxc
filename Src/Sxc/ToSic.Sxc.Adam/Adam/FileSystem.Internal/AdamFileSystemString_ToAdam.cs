using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Adam.FileSystem.Internal;

public partial class AdamFileSystemString
{

    private IFile ToAdamFile(string path)
    {
        var physicalPath = AdamPaths.PhysicalPath(path);
        var f = new FileInfo(physicalPath);
        var directoryName = f.Directory.Name;

        // todo: unclear if we need both, but we need the url for the compare-if-same-path
        var relativePath = AdamPaths.RelativeFromAdam(path);
        var relativeUrl = relativePath.ForwardSlash();
        return new File<string, string>(AdamManager)
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
            Url = AdamPaths.Url(relativeUrl),
            PhysicalPath = physicalPath,
        };
    }

    private IFolder ToAdamFolder(string path)
    {
        var physicalPath = AdamPaths.PhysicalPath(path);
        var f = new DirectoryInfo(physicalPath);

        var relativePath = AdamPaths.RelativeFromAdam(path);
        return new Folder<string, string>(AdamManager)
        {
            Path = relativePath,
            SysId = relativePath,
            ParentSysId = FindParentUrl(path),
            Name = f.Name,
            Created = f.CreationTime,
            Modified = f.LastWriteTime,

            Url = AdamPaths.Url(relativePath),
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