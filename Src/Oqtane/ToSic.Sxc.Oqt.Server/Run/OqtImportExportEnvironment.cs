using Microsoft.Data.SqlClient;
using Oqtane.Models;
using Oqtane.Repository;
using System;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Persistence.Logging;
using ToSic.Sxc.Integration;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using IO = System.IO;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtImportExportEnvironment(
    SxcImportExportEnvironmentBase.MyServices services,
    IServerPaths oqtServerPaths,
    IFileRepository oqtFileRepository,
    IFolderRepository oqtFolderRepository)
    : SxcImportExportEnvironmentBase(services, $"{OqtConstants.OqtLogPrefix}.IExEnv")
{
    /// <inheritdoc />
    /// <summary>
    /// Copy all files from SourceFolder to DestinationFolder
    /// </summary>
    /// <param name="sourceFolder"></param>
    /// <param name="destinationFolder">The portal-relative path where the files should be copied to</param>
    public override List<Message> TransferFilesToSite(string sourceFolder, string destinationFolder)
    {
        var l = Log.Fn<List<Message>>($"{sourceFolder}, {destinationFolder}");
        var messages = new List<Message>();
        var files = IO.Directory.GetFiles(sourceFolder, "*.*");
        var siteId = Site.Id;
        var oqtSite = (OqtSite)Site;

        // Ensure trim prefixSlash and backslash at the end of folder path, because Oqtane require path like that.
        destinationFolder = destinationFolder.EnsureOqtaneFolderFormat();

        var destinationVirtualPath = IO.Path.Combine(oqtSite.ContentPath, destinationFolder);
        var destinationFolderFullPath = oqtServerPaths.FullContentPath(destinationVirtualPath);

        if (!IO.Directory.Exists(destinationFolderFullPath))
        {
            Log.A($"Must create {destinationFolder} in site {siteId}");
            IO.Directory.CreateDirectory(destinationFolderFullPath);
            AddFolder(destinationFolder);
        }

        var folderInfo = oqtFolderRepository.GetFolder(siteId, destinationFolder.EnsureOqtaneFolderFormat());

        void MassLog(string msg, Exception exception)
        {
            Log.A(msg);
            if (exception == null) return;
            messages.Add(new(msg, Message.MessageTypes.Warning));
            //Exceptions.LogException(exception);
        }

        foreach (var sourceFilePath in files)
        {
            var destinationFileName = IO.Path.GetFileName(sourceFilePath);
            Log.A($"Try to copy '{sourceFilePath}' to '{destinationFileName}'");

            if (!FileExists(folderInfo, destinationFileName))
            {
                try
                {
                    using var stream = IO.File.OpenRead(sourceFilePath);
                    var fileInfo = Add(folderInfo, stream, destinationFileName, oqtSite);
                    MassLog($"Transferred '{destinationFileName}', file id is now {fileInfo?.FileId}", null);
                }
                catch (Exception e)
                {
                    MassLog($"Error: Can't copy '{destinationFileName}' because of an unknown error. " +
                            "It's likely that your files and folders are not in sync with Oqtane.", e);
                }
            }
            else
                messages.Add(new("File '" + destinationFileName + "' not copied because it already exists", Message.MessageTypes.Warning));
        }

        // Call the method recursively to handle subdirectories
        foreach (var sourceFolderPath in IO.Directory.GetDirectories(sourceFolder))
        {
            Log.A($"subfolder:{sourceFolderPath}");
            var newDestinationFolder = IO.Path.Combine(destinationFolder, sourceFolderPath.Replace(sourceFolder, "").EnsureOqtaneFolderFormat());

            TransferFilesToSite(sourceFolderPath, newDestinationFolder);
        }

        return l.Return(messages);
    }

    public override Version TenantVersion => typeof(OqtImportExportEnvironment).Assembly.GetName().Version;

    public override void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap
    ) => Log.Do($"files: {filesAndPaths.Count}, map size: {fileIdMap.Count}", l =>
    {
        foreach (var file in filesAndPaths)
        {
            var fileId = file.Key;
            var relativePath = file.Value;

            var fileName = IO.Path.GetFileName(relativePath);
            var directory = IO.Path.GetDirectoryName(relativePath).EnsureOqtaneFolderFormat();
            if (directory == null)
            {
                l.A($"Warning: File '{relativePath}', folder is 'null' doesn't exist on drive");
                continue;
            }

            if (!FolderExists(directory))
            {
                l.A($"Warning: File '{relativePath}', folder '{directory}' doesn't exist in file system");
                continue;
            }

            var folderInfo = GetOqtFolderByPath(directory);
            if (folderInfo == null)
            {
                l.A($"Warning: File '{relativePath}', folder doesn't exist in Oqtane DB");
                continue;
            }

            if (!FileExists(folderInfo, fileName))
            {
                l.A(
                    $"Warning: File '{relativePath}', file '{fileName}' doesn't exist in folder #{folderInfo.FolderId} '{folderInfo.Name}' Oqtane DB");
                continue;
            }

            var fileInfo = GetFile(folderInfo, fileName);
            if (fileInfo == null)
            {
                l.A($"Warning: File '{relativePath}', file doesn't exist in Oqtane DB (2nd check)");
                continue;
            }

            fileIdMap.Add(fileId, fileInfo.FileId);
            l.A($"Map: {fileId} will be {fileInfo.FileId} ({relativePath})");
        }

    });

    public override void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog) 
    {
        var l = Log.Fn($"folders and paths: {foldersAndPath.Count}");
        foreach (var folder in foldersAndPath)
            try
            {
                if (string.IsNullOrEmpty(folder.Value))
                {
                    l.A($"{folder.Key} / {folder.Value} is empty");
                    continue;
                }

                var directory = IO.Path.GetDirectoryName(folder.Value);
                if (directory == null)
                {
                    l.A($"Parent folder of folder {folder.Value} doesn't exist");
                    continue;
                }

                // if not exist, create - important because we need for metadata assignment
                var exists = FolderExists(directory);
                var folderInfo = !exists
                    ? AddFolder(directory)
                    : GetOqtFolderByPath(directory);

                folderIdCorrectionList.Add(folder.Key, folderInfo.FolderId);
                l.A($"Folder original #{folder.Key}/{folder.Value} - exists:{exists} in folder #{folderInfo.FolderId}");
            }
            catch (Exception)
            {
                var msg = $"Had a problem with folder of '{folder.Key}' path '{folder.Value}' - you'll have to figure out yourself if this is a problem";
                l.A(msg);
                importLog.Add(new(msg, Message.MessageTypes.Warning));
            }

        l.Done($"done - final count {folderIdCorrectionList.Count}");
    }

    private File Add(Folder parent, IO.Stream body, string fileName, OqtSite oqtSite)
    {
        var callLog = Log.Fn<File>($"Add {fileName}, folderId:{parent.FolderId}, siteId {oqtSite.Id}");

        var fullContentPath = IO.Path.Combine(oqtServerPaths.FullContentPath(oqtSite.ContentPath), parent.Path);
        IO.Directory.CreateDirectory(fullContentPath);
        var filePath = IO.Path.Combine(fullContentPath, fileName);
        using (var stream = new IO.FileStream(filePath, IO.FileMode.Create))
        {
            body.CopyTo(stream);
        }
        var fileInfo = new IO.FileInfo(filePath);

        // register into oqtane
        var oqtFileData = new File
        {
            Name = IO.Path.GetFileName(fileName),
            FolderId = parent.FolderId,
            Extension = fileInfo.Extension.ToLowerInvariant().Replace(".", ""),
            Size = (int)fileInfo.Length,
            ImageHeight = 0,
            ImageWidth = 0
        };
        var oqtFile = oqtFileRepository.AddFile(oqtFileData);
        return callLog.ReturnAsOk(oqtFile);
    }

    private bool FolderExists(string path) => GetOqtFolderByPath(path) != null;

    private bool FileExists(Folder folderInfo, string fileName) => GetFile(folderInfo, fileName) != null;

    private File GetFile(Folder folderInfo, string fileName)
        => oqtFileRepository.GetFiles(folderInfo.FolderId, false)
            .FirstOrDefault(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

    private Folder GetOqtFolderByPath(string path) => oqtFolderRepository.GetFolder(Site.Id, path.EnsureOqtaneFolderFormat());

    private Folder AddFolder(string path)
    {
        path = path.EnsureOqtaneFolderFormat();
        var callLog = Log.Fn<Folder>(path);

        if (FolderExists(path)) return callLog.ReturnNull("error, missing folder");

        try
        {
            // find parent
            var pathWithPretendFileName = path.TrimLastSlash();
            var parent = IO.Path.GetDirectoryName(pathWithPretendFileName) + "/";
            var subfolder = IO.Path.GetFileName(pathWithPretendFileName);
            var parentFolder = GetOqtFolderByPath(parent) ?? GetOqtFolderByPath("");

            // Create the new virtual folder
            var newFolder = CreateVirtualFolder(parentFolder, path, subfolder);
            return callLog.ReturnAsOk(newFolder);
        }
        catch (SqlException)
        {
            // don't do anything - this happens when multiple processes try to add the folder at the same time
            // like when two fields in a dialog cause the web-api to create the folders in parallel calls
            // see also https://github.com/2sic/2sxc/issues/811
            Log.A("error in SQL, probably folder already exists");
        }
        catch (NullReferenceException)
        {
            // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
            Log.A("error, probably folder already exists");
        }

        return callLog.ReturnNull("?");
    }

    private Folder CreateVirtualFolder(Folder parentFolder, string path, string folder)
    {
        var newVirtualFolder = AdamFolderHelper.NewVirtualFolder(Site.Id, parentFolder.FolderId, path, folder);
        oqtFolderRepository.AddFolder(newVirtualFolder);
        return newVirtualFolder;
    }
}