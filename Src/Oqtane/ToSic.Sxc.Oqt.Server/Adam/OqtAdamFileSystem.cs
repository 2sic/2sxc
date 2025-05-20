using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.FileSystem.Internal;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Paths.Internal;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using File = Oqtane.Models.File;

namespace ToSic.Sxc.Oqt.Server.Adam;

internal class OqtAdamFileSystem(
    IFileRepository oqtFileRepository,
    IFolderRepository oqtFolderRepository,
    IServerPaths serverPaths,
    IAdamPaths adamPaths)
    : AdamFileSystemBase(adamPaths, OqtConstants.OqtLogPrefix, [serverPaths, oqtFileRepository, oqtFolderRepository]),
        IAdamFileSystem
{
    public IFileRepository OqtFileRepository { get; } = oqtFileRepository;
    public IFolderRepository OqtFolderRepository { get; } = oqtFolderRepository;

    #region Files

    public override IFile GetFile(AdamAssetIdentifier fileId)
    {
        var id = ((AdamAssetId<int>)fileId).SysId;
        var file = OqtFileRepository.GetFile(id);
        return OqtToAdam(file);
    }

    public override void Rename(IFile file, string newName)
    {
        var l = Log.Fn();
        try
        {
            var path = serverPaths.FullContentPath(file.Path);

            var currentFilePath = Path.Combine(path, file.FullName);
            if (!FsHelpers.TryToRenameFile(currentFilePath, newName))
            {
                l.Done("");
                return;
            }

            var oqtFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
            oqtFile.Name = newName;
            OqtFileRepository.UpdateFile(oqtFile);
            l.A($"VirtualFile {oqtFile.FileId} renamed to {oqtFile.Name}");

            l.Done("ok");
        }
        catch (Exception e)
        {
            l.Done($"Error:{e.Message}; {e.InnerException}");
        }
    }

    public override void Delete(IFile file)
    {
        var l = Log.Fn();
        var oqtFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
        OqtFileRepository.DeleteFile(oqtFile.FileId);
        l.Done();
    }

    public override IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
    {
        var l = Log.Fn<IFile>($"..., ..., {fileName}, {ensureUniqueName}");
        if (ensureUniqueName)
            fileName = FindUniqueFileName(parent, fileName);
        var fullContentPath = serverPaths.FullContentPath(parent.Path);
        Directory.CreateDirectory(fullContentPath);
        var filePath = Path.Combine(fullContentPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            body.CopyTo(stream);
        }
        var fileInfo = new FileInfo(filePath);

        // register into oqtane
        var oqtFileData = new File
        {
            Name = Path.GetFileName(fileName),
            FolderId = parent.Id,
            Extension = fileInfo.Extension.ToLowerInvariant().Replace(".", ""),
            Size = (int)fileInfo.Length,
            ImageHeight = 0,
            ImageWidth = 0
        };
        var oqtFile = OqtFileRepository.AddFile(oqtFileData);
        var file = GetFile(AdamAssetIdentifier.Create(oqtFile.FileId));
        return l.ReturnAsOk(file);
    }

    /// <summary>
    /// When uploading a new file, we must verify that the name isn't used.
    /// If it is used, walk through numbers to make a new name which isn't used.
    /// </summary>
    /// <param name="parentFolder"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private string FindUniqueFileName(IFolder parentFolder, string fileName)
    {
        var l = Log.Fn<string>($"..., {fileName}");

        var oqtFolder = OqtFolderRepository.GetFolder(parentFolder.AsOqt().SysId);
        var serverPath = Path.Combine(serverPaths.FullContentPath(AdamManager.Site.ContentPath), oqtFolder.Path);

        return l.Return(FsHelpers.FindUniqueFileName(serverPath, fileName));
    }

    #endregion



    #region Folders


    public override bool FolderExists(string path) => GetOqtFolderByName(path) != null;

    private Folder GetOqtFolderByName(string path) => OqtFolderRepository.GetFolder(AdamManager.Site.Id, path.EnsureOqtaneFolderFormat());

    public override void AddFolder(string path) => Log.Do(() =>
    {
        path = path.Backslash();
        if (FolderExists(path)) return "";

        try
        {
            // find parent
            var pathWithPretendFileName = path.TrimLastSlash();
            var parent = Path.GetDirectoryName(pathWithPretendFileName) + "/";
            var subfolder = Path.GetFileName(pathWithPretendFileName);
            var parentFolder = GetOqtFolderByName(parent) ?? GetOqtFolderByName("");

            // Create the new virtual folder
            CreateVirtualFolder(parentFolder, path, subfolder);
            return "ok";
        }
        catch (SqlException)
        {
            // don't do anything - this happens when multiple processes try to add the folder at the same time
            // like when two fields in a dialog cause the web-api to create the folders in parallel calls
            // see also https://github.com/2sic/2sxc/issues/811
            return "error in SQL, probably folder already exists";
        }
        catch (DbUpdateException)
        {
            return $"error in EF, probably folder already exists";
        }
        catch (NullReferenceException)
        {
            // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
            return "error, probably folder already exists";
        }
    });

    private Folder CreateVirtualFolder(Folder parentFolder, string path, string folder)
    {
        var newVirtualFolder = AdamFolderHelper.NewVirtualFolder(AdamManager.Site.Id, parentFolder.FolderId, path, folder);
        OqtFolderRepository.AddFolder(newVirtualFolder);
        return newVirtualFolder;
    }

    public override void Rename(IFolder folder, string newName) => Log.Do($"..., {newName}", () =>
    {
        var fld = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
        WipConstants.AdamNotImplementedYet();
        Log.A("Not implement yet in Oqtane");
    });

    public override void Delete(IFolder folder) => Log.Do(() => OqtFolderRepository.DeleteFolder(folder.AsOqt().SysId));

    public override IFolder Get(string path) => OqtToAdam(GetOqtFolderByName(path));


    public override List<IFolder> GetFolders(IFolder folder)
    {
        var l = Log.Fn<List<IFolder>>();
        var fldObj = GetOqtFolder(folder.AsOqt().SysId);
        if (fldObj == null) return [];

        var firstList = GetSubFoldersRecursive(fldObj);
        var folders = firstList?.Select(OqtToAdam).ToList()
                      ?? [];
        return l.Return(folders, $"{folders.Count}");
    }

    private List<Folder> GetSubFoldersRecursive(Folder parentFolder, List<Folder> allFolders = null, List<Folder> subFolders = null)
    {
        allFolders ??= OqtFolderRepository.GetFolders(parentFolder.SiteId).ToList();
        subFolders ??= [];
        allFolders.Where(f => f.ParentId == parentFolder.FolderId).ToList().ForEach(f =>
        {
            subFolders.Add(f);
            GetSubFoldersRecursive(f, allFolders, subFolders);
        });
        return subFolders;
    }

    //public override Folder<int, int> GetFolder(int folderId)
    //    => OqtToAdam(GetOqtFolder(folderId));

    public override IFolder GetFolder(AdamAssetIdentifier folderId)
        => OqtToAdam(GetOqtFolder(((AdamAssetId<int>)folderId).SysId));

    #endregion

    #region Oqtane typed calls

    private Folder GetOqtFolder(int folderId) => OqtFolderRepository.GetFolder(folderId);


    public override List<IFile> GetFiles(IFolder folder)
    {
        var l = Log.Fn<List<IFile>>();
        var fldObj = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
        // sometimes the folder doesn't exist for whatever reason
        if (fldObj == null) return [];

        // try to find the files
        var firstList = OqtFileRepository.GetFiles(fldObj.FolderId);
        var files = firstList?.Select(OqtToAdam).ToList()
                    ?? [];
        return l.Return(files, $"{files.Count}");
    }

    #endregion

    #region OqtToAdam

    //public string GetUrl(string folderPath) => _adamPaths.Url(folderPath.ForwardSlash());

    private IFolder OqtToAdam(Folder f) =>
        new Folder<int, int>(AdamManager)
        {
            Path = ((OqtAdamPaths)AdamPaths).Path(f.Path),
            SysId = f.FolderId,

            ParentSysId = f.ParentId ?? WipConstants.ParentFolderNotFound,

            Name = f.Name,
            Created = f.CreatedOn,
            Modified = f.ModifiedOn,
            Url = GetUrl(f.Path), // _adamPaths.Url(f.Path.ForwardSlash()),
            PhysicalPath = AdamPaths.PhysicalPath(f.Path),
        };



    private IFile OqtToAdam(File f) =>
        new File<int, int>(AdamManager)
         {
             FullName = f.Name,
             Extension = f.Extension,
             Size = f.Size,
             SysId = f.FileId,
             Folder = f.Folder.Name,
             ParentSysId = f.FolderId,

             Path = ((OqtAdamPaths)AdamPaths).Path(f.Folder.Path),

             Created = f.CreatedOn,
             Modified = f.ModifiedOn,
             Name = Path.GetFileNameWithoutExtension(f.Name),
             Url = AdamPaths.Url(Path.Combine(f.Folder.Path, f.Name).ForwardSlash()),
             PhysicalPath = AdamPaths.PhysicalPath(Path.Combine(f.Folder.Path, f.Name)),
         };


    #endregion
}