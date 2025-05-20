namespace ToSic.Sxc.Adam.Internal;

public partial class AdamFileSystemString
{
    public override IFile GetFile(AdamAssetIdentifier fileId)
    {
        var id = ((AdamAssetId<string>)fileId).SysId;
        var dir = FsHelpers.EnsurePhysicalPath(id);
        return ToAdamFile(dir);
    }

    /// <inheritdoc />
    public override List<IFile> GetFiles(IFolder folder)
    {
        var dir = Directory.GetFiles(FsHelpers.EnsurePhysicalPath(folder.Path));
        return dir.Select(ToAdamFile).ToList();
    }


    /// <inheritdoc />
    public override IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
    {
        var l = Log.Fn<IFile>($"..., ..., {fileName}, {ensureUniqueName}");
        if (ensureUniqueName)
            fileName = FsHelpers.FindUniqueFileName(AdamPaths.PhysicalPath(parent.Path), fileName);
        var fullContentPath = AdamPaths.PhysicalPath(parent.Path);
        Directory.CreateDirectory(fullContentPath);
        var filePath = Path.Combine(fullContentPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        body.CopyTo(stream);
        var fileInfo = GetFile(AdamAssetIdentifier.Create(filePath));

        return l.ReturnAsOk(fileInfo);
    }
        
}