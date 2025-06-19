using DotNetNuke.Services.FileSystem;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Configuration;
using ToSic.Eav.Apps.Sys;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.FileSystem.Internal;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Manager.Internal;

namespace ToSic.Sxc.Dnn.Adam;

internal class DnnAdamFileSystem() : ServiceBase("Dnn.FilSys"), IAdamFileSystem
{
    #region Constructor / DI / Init

    public void Init(AdamManager adamManager)
    {
        var l = Log.Fn();
        AdamManager = adamManager;
        l.Done();
    }

    protected AdamManager AdamManager;

    #endregion

    #region FileSystem Settings

    public int MaxUploadKb()
        => (ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection)
            ?.MaxRequestLength ?? 1;

    #endregion

    #region Files
    private readonly IFileManager _dnnFiles = FileManager.Instance;

    //public File<int, int> GetFile(int fileId)
    //{
    //    var file = _dnnFiles.GetFile(fileId);
    //    return DnnToAdam(file);
    //}

    public IFile GetFile(AdamAssetIdentifier fileId)
    {
        var id = ((AdamAssetId<int>)fileId).SysId;
        var file = _dnnFiles.GetFile(id);
        return DnnToAdam(file);
    }

    public void Rename(IFile file, string newName)
    {
        var l = Log.Fn($"{nameof(file)}:{file.Id}, {nameof(newName)}: {newName}");
        var dnnFile = _dnnFiles.GetFile(file.AsDnn().SysId);
        _dnnFiles.RenameFile(dnnFile, newName);
        l.Done();
    }

    public void Delete(IFile file)
    {
        var l = Log.Fn($"file: {file.Id}", timer: true);
        var dnnFile = _dnnFiles.GetFile(file.AsDnn().SysId);
        // 2025-06 For unknown reasons this suddenly breaks; same DNN, some 2sxc code
        // Says file is in use, but if we debug-step-through, it works; seems to be timing
        Retry.RetryOnException(() => _dnnFiles.DeleteFile(dnnFile), l, repeat: 10, delay: 200, silent: false);

        l.Done();
    }

    public IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
    {
        var l = Log.Fn<IFile>($"..., {fileName}, {ensureUniqueName}");
        if (ensureUniqueName) fileName = FindUniqueFileName(parent, fileName);
        var dnnFolder = _dnnFolders.GetFolder(parent.AsDnn().SysId);
        var dnnFile = _dnnFiles.AddFile(dnnFolder, Path.GetFileName(fileName), body);
        var file = GetFile(AdamAssetIdentifier.Create(dnnFile.FileId));
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
        var dnnFolder = _dnnFolders.GetFolder(parentFolder.AsDnn().SysId);
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName);
        for (var i = 1;
             i < AdamConstants.MaxSameFileRetries &&
             _dnnFiles.FileExists(dnnFolder, Path.GetFileName(fileName));
             i++)
            fileName = $"{name}-{i}{ext}";

        return l.ReturnAsOk(fileName);
    }

    #endregion



    #region Folders
        
    private readonly IFolderManager _dnnFolders = FolderManager.Instance;

    public bool FolderExists(string path)
    {
        var l = Log.Fn<bool>($"path:{path}");
        return l.ReturnAsOk(_dnnFolders.FolderExists(AdamManager.Site.Id, path));
    } 
        

    public void AddFolder(string path)
    {
        var l = Log.Fn($"path:{path}");
        try
        {
            _dnnFolders.AddFolder(AdamManager.Site.Id, path);
            l.Done("ok");
        }
        catch (SqlException)
        {
            // don't do anything - this happens when multiple processes try to add the folder at the same time
            // like when two fields in a dialog cause the web-api to create the folders in parallel calls
            // see also https://github.com/2sic/2sxc/issues/811
            l.Done("error in DNN SQL, probably folder already exists");
        }
        catch (FolderAlreadyExistsException)
        {
            // Dnn reports it already exists - it shouldn't have got here because that was checked before
            // but I guess depending on the DNN version this isn't 100% reliable
            l.Done("Dnn says folder already exists");
        }
        catch (NullReferenceException)
        {
            // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
            l.Done("error, probably folder already exists");
        }
    }

    public void Rename(IFolder folder, string newName)
    {
        var l = Log.Fn($"folder:{folder.Id}, newName:{newName}");
        var fld = _dnnFolders.GetFolder(folder.AsDnn().SysId);
        _dnnFolders.RenameFolder(fld, newName);
        l.Done();
    }

    public void Delete(IFolder folder)
    {
        var l = Log.Fn($"folder:{folder.Id}");
        _dnnFolders.DeleteFolder(folder.AsDnn().SysId);
        l.Done();
    }


    public IFolder Get(string path)
    {
        var l = Log.Fn<IFolder>($"path:{path}");
        return l.ReturnAsOk(DnnToAdam(_dnnFolders.GetFolder(AdamManager.Site.Id, path)));
    }


    public List<IFolder> GetFolders(IFolder folder)
    {
        var l = Log.Fn<List<IFolder>>($"folder:{folder.Id}");
        var fldObj = GetDnnFolder(folder.AsDnn().SysId);
        if (fldObj == null) return l.Return([], "");

        var firstList = _dnnFolders.GetFolders(fldObj);
        var folders = firstList?.Select(DnnToAdam).ToList()
                      ?? [];
        return l.Return(folders, $"{folders.Count}");
    }

    //public Folder<int, int> GetFolder(int folderId)
    //    => DnnToAdam(GetDnnFolder(folderId));

    public IFolder GetFolder(AdamAssetIdentifier folderId)
        => DnnToAdam(GetDnnFolder(((AdamAssetId<int>)folderId).SysId));


    #endregion




    #region Dnn typed calls

    private IFolderInfo GetDnnFolder(int folderId) => _dnnFolders.GetFolder(folderId);


    public List<IFile> GetFiles(IFolder folder)
    {
        var l = Log.Fn<List<IFile>>($"folder:{folder.Id}");
        var fldObj = _dnnFolders.GetFolder(folder.AsDnn().SysId);
        // sometimes the folder doesn't exist for whatever reason
        if (fldObj == null)
            return l.Return([], "");

        // try to find the files
        var firstList = _dnnFolders.GetFiles(fldObj);
        var files = firstList?.Select(DnnToAdam).ToList()
                    ?? [];
        return l.Return(files, $"{files.Count}");
    }

    #endregion

    #region DnnToAdam

    private const string ErrorDnnObjectNull = "Tried to create Adam object but the original is invalid. " +
                                              "Probably the DNN file system is out of sync. " +
                                              "Re-Sync in the DNN files recursively (in Admin - Files) and the error should go away. ";

    public string GetUrl(string folderPath) => AdamManager.Site.ContentPath + folderPath;

    private /*Folder<int, int>*/IFolder DnnToAdam(IFolderInfo dnnFolderInfo)
    {
        var l = Log.Fn<Folder<int, int>>($"folderName: {dnnFolderInfo.FolderName}");

        if (dnnFolderInfo == null)
            throw l.Done(new ArgumentNullException(nameof(dnnFolderInfo), ErrorDnnObjectNull));

        var folder = new Folder<int, int>(AdamManager)
        {
            Path = dnnFolderInfo.FolderPath,
            SysId = dnnFolderInfo.FolderID,

            ParentSysId = dnnFolderInfo.ParentID,

            Name = dnnFolderInfo.DisplayName,
            Created = dnnFolderInfo.CreatedOnDate,
            Modified = dnnFolderInfo.LastModifiedOnDate,
            Url = GetUrl(dnnFolderInfo.FolderPath), // AdamManager.Site.ContentPath + dnnFolderInfo.FolderPath,
            PhysicalPath = dnnFolderInfo.PhysicalPath,
        };
        return l.ReturnAsOk(folder);
    }


    private /*File<int, int>*/IFile DnnToAdam(IFileInfo dnnFileInfo)
    {
        var l = Log.Fn<File<int, int>>($"fileName: {dnnFileInfo.FileName}");
            
        if (dnnFileInfo == null)
            throw l.Done(new ArgumentNullException(nameof(dnnFileInfo), ErrorDnnObjectNull));

        return l.ReturnAsOk(new File<int, int>(AdamManager)
        {
            FullName = dnnFileInfo.FileName,
            Extension = dnnFileInfo.Extension,
            Size = dnnFileInfo.Size,
            SysId = dnnFileInfo.FileId,
            Folder = dnnFileInfo.Folder,
            ParentSysId = dnnFileInfo.FolderId,

            Path = dnnFileInfo.RelativePath,

            Created = dnnFileInfo.CreatedOnDate,
            Modified = dnnFileInfo.LastModifiedOnDate,
            Name = Path.GetFileNameWithoutExtension(dnnFileInfo.FileName),
            Url = dnnFileInfo.StorageLocation == 0
                ? AdamManager.Site.ContentPath + dnnFileInfo.Folder + dnnFileInfo.FileName
                : FileLinkClickController.Instance.GetFileLinkClick(dnnFileInfo),
            PhysicalPath = dnnFileInfo.PhysicalPath,
        });
    }

    #endregion
}