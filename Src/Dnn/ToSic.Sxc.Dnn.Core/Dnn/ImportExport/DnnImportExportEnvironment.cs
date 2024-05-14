using System.IO;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav.Persistence.Logging;
using ToSic.Sxc.Integration;

namespace ToSic.Sxc.Dnn.ImportExport;

internal class DnnImportExportEnvironment : SxcImportExportEnvironmentBase
{
    #region Constructors

    /// <summary>
    /// DI Constructor
    /// </summary>
    public DnnImportExportEnvironment(MyServices services) : base(services, "Dnn.ImExEn") { }

    #endregion

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
        var files = Directory.GetFiles(sourceFolder, "*.*");

        var dnnFileManager = FileManager.Instance;
        var dnnFolderManager = FolderManager.Instance;
        var siteId = Site.Id;

        if (!dnnFolderManager.FolderExists(siteId, destinationFolder))
        {
            l.A($"Must create {destinationFolder} in site {siteId}");
            dnnFolderManager.AddFolder(siteId, destinationFolder);
        }
        var folderInfo = dnnFolderManager.GetFolder(siteId, destinationFolder);

        void MassLog(string msg, Exception exception)
        {
            l.A(msg);
            if (exception == null) return;
            messages.Add(exception is InvalidFileExtensionException
                ? new(msg, Message.MessageTypes.Error)
                : new Message(msg, Message.MessageTypes.Warning));

            Exceptions.LogException(exception);
        }

        foreach (var sourceFilePath in files)
        {
            var destinationFileName = Path.GetFileName(sourceFilePath);
            l.A($"Try to copy '{sourceFilePath}' to '{destinationFileName}'");

            if (!dnnFileManager.FileExists(folderInfo, destinationFileName))
            {
                try
                {
                    using var stream = File.OpenRead(sourceFilePath);
                    var fileInfo = dnnFileManager.AddFile(folderInfo, destinationFileName, stream, false);
                    MassLog($"Transferred '{destinationFileName}', dnn-id is now {fileInfo?.FileId}", null);
                }
                catch (InvalidFileExtensionException e)
                {
                    MassLog($"Error: '{destinationFileName}' not copied because the file extension is not allowed.", e);
                }
                catch (Exception e)
                {
                    MassLog($"Error: Can't copy '{destinationFileName}' because of an unknown error. " +
                            "It's likely that your files and folders are not in sync with DNN, usually re-syncing will fix the issue.", e);
                }
            }
            else
                messages.Add(new("File '" + destinationFileName + "' not copied because it already exists", Message.MessageTypes.Warning));
        }

        // Call the method recursively to handle subdirectories
        foreach (var sourceFolderPath in Directory.GetDirectories(sourceFolder))
        {
            l.A($"subfolder:{sourceFolderPath}");
            var newDestinationFolder = Path.Combine(destinationFolder, sourceFolderPath.Replace(sourceFolder, "")
                    .TrimStart('\\'))
                .Replace('\\', '/');
            TransferFilesToSite(sourceFolderPath, newDestinationFolder);
        }

        return l.ReturnAsOk(messages);
    }

    public override Version TenantVersion => typeof(PortalSettings).Assembly.GetName().Version;

    #region stuff we need for Import

    public override void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap)
    {
        var l = Log.Fn($"files: {filesAndPaths.Count}, map size: {fileIdMap.Count}");
        var siteId = Site.Id;
        var fileManager = FileManager.Instance;
        var folderManager = FolderManager.Instance;

        foreach (var file in filesAndPaths)
        {
            var fileId = file.Key;
            var relativePath = file.Value;

            var fileName = Path.GetFileName(relativePath);
            var directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/');
            if (directory == null)
            {
                l.A($"Warning: File '{relativePath}', folder doesn't exist on drive");
                continue;
            }

            if (!folderManager.FolderExists(siteId, directory))
            {
                l.A($"Warning: File '{relativePath}', folder doesn't exist in DNN DB");
                continue;
            }

            var folderInfo = folderManager.GetFolder(siteId, directory);

            if (!fileManager.FileExists(folderInfo, fileName))
            {
                l.A($"Warning: File '{relativePath}', file doesn't exist in DNN DB");
                continue;
            }

            var fileInfo = fileManager.GetFile(folderInfo, fileName);
            fileIdMap.Add(fileId, fileInfo.FileId);
            l.A($"Map: {fileId} will be {fileInfo.FileId} ({relativePath})");
        }

        l.Done();
    }

    public override void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog) 
    {
        var l = Log.Fn($"folders and paths: {foldersAndPath.Count}");
        var siteId = Site.Id;
        var folderManager = FolderManager.Instance;

        foreach (var folder in foldersAndPath)
            try
            {
                if (string.IsNullOrEmpty(folder.Value))
                {
                    l.A($"{folder.Key} / {folder.Value} is empty");
                    continue;
                }

                var directory = Path.GetDirectoryName(folder.Value)?.Replace('\\', '/');
                if (directory == null)
                {
                    l.A($"Parent folder of folder {folder.Value} doesn't exist");
                    continue;
                }

                // if not exist, create - important because we need for metadata assignment
                var exists = folderManager.FolderExists(siteId, directory);
                var folderInfo = !exists
                    ? folderManager.AddFolder(siteId, directory)
                    : folderManager.GetFolder(siteId, directory);

                folderIdCorrectionList.Add(folder.Key, folderInfo.FolderID);
                l.A($"Folder original #{folder.Key}/{folder.Value} - exists:{exists} placed in folder #{folderInfo.FolderID}");
            }
            catch (Exception)
            {
                var msg = $"Had problem with folder '{folder.Key}' path '{folder.Value}' - you'll have to figure out yourself if this is a problem";
                l.A(msg);
                importLog.Add(new(msg, Message.MessageTypes.Warning));
            }

        l.Done($"done - final count {folderIdCorrectionList.Count}");
    }

    #endregion

}