using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using Oqtane.Repository;
using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using File = Oqtane.Models.File;

namespace ToSic.Sxc.Oqt.Server.Adam;

internal class OqtAdamFileSystem : AdamFileSystemBasic<int, int>, IAdamFileSystem<int, int>
{
    private readonly IServerPaths _serverPaths;
    public IFileRepository OqtFileRepository { get; }
    public IFolderRepository OqtFolderRepository { get; }

    #region Constructor / DI / Init

    public OqtAdamFileSystem(IFileRepository oqtFileRepository, IFolderRepository oqtFolderRepository, IServerPaths serverPaths, IAdamPaths adamPaths)
        : base(adamPaths, OqtConstants.OqtLogPrefix)
    {
        ConnectLogs([
            _serverPaths = serverPaths,
            OqtFileRepository = oqtFileRepository,
            OqtFolderRepository = oqtFolderRepository
        ]);
    }

    #endregion

    #region FileSystem Settings

    //public int MaxUploadKb()
    //    => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)
    //       ?.MaxRequestLength ?? 1;

    #endregion

    #region Files

    public override File<int, int> GetFile(int fileId)
    {
        var file = OqtFileRepository.GetFile(fileId);
        return OqtToAdam(file);
    }

    public override void Rename(IFile file, string newName) => Log.Do(l =>
    {
        try
        {
            var path = _serverPaths.FullContentPath(file.Path);

            var currentFilePath = Path.Combine(path, file.FullName);
            if (!FsHelpers.TryToRenameFile(currentFilePath, newName)) return "";

            var oqtFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
            oqtFile.Name = newName;
            OqtFileRepository.UpdateFile(oqtFile);
            l.A($"VirtualFile {oqtFile.FileId} renamed to {oqtFile.Name}");

            return "ok";
        }
        catch (Exception e)
        {
            return $"Error:{e.Message}; {e.InnerException}";
        }
    });

    public override void Delete(IFile file)
    {
        var l = Log.Fn();
        var oqtFile = OqtFileRepository.GetFile(file.AsOqt().SysId);
        OqtFileRepository.DeleteFile(oqtFile.FileId);
        l.Done();
    }

    public override File<int, int> Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
    {
        var callLog = Log.Fn<File<int, int>>($"..., ..., {fileName}, {ensureUniqueName}");
        if (ensureUniqueName)
            fileName = FindUniqueFileName(parent, fileName);
        var fullContentPath = _serverPaths.FullContentPath(parent.Path);
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
        return callLog.ReturnAsOk(GetFile(oqtFile.FileId));
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
        var serverPath = Path.Combine(_serverPaths.FullContentPath(AdamManager.Site.ContentPath), oqtFolder.Path);

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

    public override Folder<int, int> Get(string path) => OqtToAdam(GetOqtFolderByName(path));

    public override List<Folder<int, int>> GetFolders(IFolder folder)
    {
        var callLog = Log.Fn<List<Folder<int, int>>>();
        var fldObj = GetOqtFolder(folder.AsOqt().SysId);
        if (fldObj == null) return [];

        var firstList = GetSubFoldersRecursive(fldObj);
        var folders = firstList?.Select(OqtToAdam).ToList()
                      ?? [];
        return callLog.Return(folders, $"{folders.Count}");
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

    public override Folder<int, int> GetFolder(int folderId) => OqtToAdam(GetOqtFolder(folderId));

    #endregion

    #region Oqtane typed calls

    private Folder GetOqtFolder(int folderId) => OqtFolderRepository.GetFolder(folderId);


    public override List<File<int, int>> GetFiles(IFolder folder)
    {
        var callLog = Log.Fn<List<File<int, int>>>();
        var fldObj = OqtFolderRepository.GetFolder(folder.AsOqt().SysId);
        // sometimes the folder doesn't exist for whatever reason
        if (fldObj == null) return [];

        // try to find the files
        var firstList = OqtFileRepository.GetFiles(fldObj.FolderId);
        var files = firstList?.Select(OqtToAdam).ToList()
                    ?? [];
        return callLog.Return(files, $"{files.Count}");
    }

    #endregion

    #region OqtToAdam

    //public string GetUrl(string folderPath) => _adamPaths.Url(folderPath.ForwardSlash());

    private Folder<int, int> OqtToAdam(Folder f)
        => new(AdamManager)
        {
            Path = ((OqtAdamPaths)_adamPaths).Path(f.Path),
            SysId = f.FolderId,

            ParentSysId = f.ParentId ?? WipConstants.ParentFolderNotFound,

            Name = f.Name,
            Created = f.CreatedOn,
            Modified = f.ModifiedOn,
            Url = GetUrl(f.Path), // _adamPaths.Url(f.Path.ForwardSlash()),
            PhysicalPath = _adamPaths.PhysicalPath(f.Path),
        };



    private File<int, int> OqtToAdam(File f)
         => new(AdamManager)
         {
             FullName = f.Name,
             Extension = f.Extension,
             Size = f.Size,
             SysId = f.FileId,
             Folder = f.Folder.Name,
             ParentSysId = f.FolderId,

             Path = ((OqtAdamPaths)_adamPaths).Path(f.Folder.Path),

             Created = f.CreatedOn,
             Modified = f.ModifiedOn,
             Name = Path.GetFileNameWithoutExtension(f.Name),
             Url = _adamPaths.Url(Path.Combine(f.Folder.Path, f.Name).ForwardSlash()),
             PhysicalPath = _adamPaths.PhysicalPath(Path.Combine(f.Folder.Path, f.Name)),
         };


    #endregion
}