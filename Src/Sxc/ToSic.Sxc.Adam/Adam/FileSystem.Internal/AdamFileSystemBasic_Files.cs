using System.IO;

namespace ToSic.Sxc.Adam.Internal;

public partial class AdamFileSystemBasic
{
    /// <inheritdoc />
    //public override File<string, string> GetFile(string fileId)
    //{
    //    var dir = FsHelpers.EnsurePhysicalPath(fileId);
    //    return ToAdamFile(dir);
    //}

    public override IFile GetFile(AdamAssetIdentifier fileId)
    {
        var id = ((AdamAssetId<string>)fileId).SysId;
        var dir = FsHelpers.EnsurePhysicalPath(id);
        return ToAdamFile(dir);
    }

    /// <inheritdoc />
    public override List<File<string, string>> GetFiles(IFolder folder)
    {
        var dir = Directory.GetFiles(FsHelpers.EnsurePhysicalPath(folder.Path));
        return dir.Select(ToAdamFile).ToList();
    }


    /// <inheritdoc />
    public override IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
    {
        var l = Log.Fn<IFile>($"..., ..., {fileName}, {ensureUniqueName}");
        if (ensureUniqueName)
            fileName = FsHelpers.FindUniqueFileName(_adamPaths.PhysicalPath(parent.Path), fileName);
        var fullContentPath = _adamPaths.PhysicalPath(parent.Path);
        Directory.CreateDirectory(fullContentPath);
        var filePath = Path.Combine(fullContentPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        body.CopyTo(stream);
        var fileInfo = GetFile(AdamAssetIdentifier.Create(filePath));

        return l.ReturnAsOk(fileInfo);
    }
        
}