namespace ToSic.Sxc.Adam.Internal;

public partial class AdamFileSystemString
{
    /// <inheritdoc />
    public override void AddFolder(string path)
    {
        path = AdamPaths.PhysicalPath(path);
        Directory.CreateDirectory(path);
    }

    /// <inheritdoc />
    public override bool FolderExists(string path)
    {
        var serverPath = AdamPaths.PhysicalPath(path);
        return Directory.Exists(serverPath);
    }

    /// <inheritdoc />
    public override IFolder GetFolder(AdamAssetIdentifier folderId)
        => ToAdamFolder(FsHelpers.EnsurePhysicalPath(((AdamAssetId<string>)folderId).SysId));

    /// <inheritdoc />
    public override List<IFolder> GetFolders(IFolder folder)
    {
        var dir = Directory.GetDirectories(FsHelpers.EnsurePhysicalPath(folder.Path));
        return dir.Select(ToAdamFolder).ToList();
    }


    /// <inheritdoc />
    public override void Rename(IFolder folder, string newName) => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Delete(IFolder folder) => throw new NotSupportedException();

    /// <inheritdoc />
    public override IFolder Get(string path) => ToAdamFolder(path);

}