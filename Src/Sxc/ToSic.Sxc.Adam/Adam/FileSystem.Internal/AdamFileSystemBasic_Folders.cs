using System.IO;

namespace ToSic.Sxc.Adam.Internal;

public partial class AdamFileSystemBasic
{
    /// <inheritdoc />
    public override void AddFolder(string path)
    {
        path = _adamPaths.PhysicalPath(path);
        Directory.CreateDirectory(path);
    }

    /// <inheritdoc />
    public override bool FolderExists(string path)
    {
        var serverPath = _adamPaths.PhysicalPath(path);
        return Directory.Exists(serverPath);
    }

    /// <inheritdoc />
    public override Folder<string, string> GetFolder(string folderId) => ToAdamFolder(FsHelpers.EnsurePhysicalPath(folderId));

    /// <inheritdoc />
    public override List<Folder<string, string>> GetFolders(IFolder folder)
    {
        var dir = Directory.GetDirectories(FsHelpers.EnsurePhysicalPath(folder.Path));
        return dir.Select(ToAdamFolder).ToList();
    }


    /// <inheritdoc />
    public override void Rename(IFolder folder, string newName) => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Delete(IFolder folder) => throw new NotSupportedException();

    /// <inheritdoc />
    public override Folder<string, string> Get(string path) => ToAdamFolder(path);

}