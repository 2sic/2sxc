﻿using Oqtane.Extensions;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;
using IO = System.IO;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtImportExportEnvironment : ImportExportEnvironmentBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<ISiteRepository> _siteRepository;
        private readonly IServerPaths _oqtServerPaths;
        private readonly IFileRepository _oqtFileRepository;
        private readonly IFolderRepository _oqtFolderRepository;

        public OqtImportExportEnvironment(Dependencies dependencies, IServiceProvider serviceProvider, Lazy<ISiteRepository> siteRepository, IServerPaths oqtServerPaths, IFileRepository oqtFileRepository, IFolderRepository oqtFolderRepository) : base(dependencies, $"{OqtConstants.OqtLogPrefix}.IExEnv")
        {
            _serviceProvider = serviceProvider;
            _siteRepository = siteRepository;
            _oqtServerPaths = oqtServerPaths;
            _oqtFileRepository = oqtFileRepository;
            _oqtFolderRepository = oqtFolderRepository;
        }

        /// <inheritdoc />
        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder">The portal-relative path where the files should be copied to</param>
        public override List<Message> TransferFilesToSite(string sourceFolder, string destinationFolder)
        {
            var wrapLog = Log.Call<List<Message>>($"{sourceFolder}, {destinationFolder}");
            var messages = new List<Message>();
            var files = IO.Directory.GetFiles(sourceFolder, "*.*");
            var siteId = Site.Id;
            var site = _siteRepository.Value.GetSite(siteId);
            var oqtSite = _serviceProvider.Build<OqtSite>().Init(site);

            // Ensure trim prefixSlash and backslash at the end of folder path, because Oqtane require path like that.
            destinationFolder = (destinationFolder.Backslash().TrimLastSlash() + IO.Path.DirectorySeparatorChar).TrimPrefixSlash();

            var destinationVirtualPath = IO.Path.Combine(oqtSite.ContentPath, destinationFolder);
            var destinationFolderFullPath = _oqtServerPaths.FullContentPath(destinationVirtualPath);

            if (!IO.Directory.Exists(destinationFolderFullPath))
            {
                Log.Add($"Must create {destinationFolder} in site {siteId}");
                IO.Directory.CreateDirectory(destinationFolderFullPath);
                AddFolder(destinationFolder);
            }

            var folderInfo = _oqtFolderRepository.GetFolder(siteId, destinationFolder);

            void MassLog(string msg, Exception exception)
            {
                Log.Add(msg);
                if (exception == null) return;
                messages.Add(new Message(msg, Message.MessageTypes.Warning));
                //Exceptions.LogException(exception);
            }

            foreach (var sourceFilePath in files)
            {
                var destinationFileName = IO.Path.GetFileName(sourceFilePath);
                Log.Add($"Try to copy '{sourceFilePath}' to '{destinationFileName}'");

                if (!FileExists(folderInfo, destinationFileName))
                {
                    try
                    {
                        using (var stream = IO.File.OpenRead(sourceFilePath))
                        {
                            var fileInfo = Add(folderInfo, stream, destinationFileName, oqtSite);
                            MassLog($"Transferred '{destinationFileName}', dnn-id is now {fileInfo?.FileId}", null);
                        }
                    }
                    catch (Exception e)
                    {
                        MassLog($"Error: Can't copy '{destinationFileName}' because of an unknown error. " +
                                  "It's likely that your files and folders are not in sync with Oqtane.", e);
                    }
                }
                else
                    messages.Add(new Message("File '" + destinationFileName + "' not copied because it already exists", Message.MessageTypes.Warning));
            }

            // Call the method recursively to handle subdirectories
            foreach (var sourceFolderPath in IO.Directory.GetDirectories(sourceFolder))
            {
                Log.Add($"subfolder:{sourceFolderPath}");
                var newDestinationFolder = IO.Path.Combine(destinationFolder, sourceFolderPath.Replace(sourceFolder, "")
                    .TrimStart('\\'))
                    .Replace('\\', '/');
                TransferFilesToSite(sourceFolderPath, newDestinationFolder);
            }

            return wrapLog(null, messages);
        }

        public override Version TenantVersion => typeof(OqtImportExportEnvironment).Assembly.GetName().Version;

        public override void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap)
        {
            var wrapLog = Log.Call($"files: {filesAndPaths.Count}, map size: {fileIdMap.Count}");

            foreach (var file in filesAndPaths)
            {
                var fileId = file.Key;
                var relativePath = file.Value;

                var fileName = IO.Path.GetFileName(relativePath);
                var directory = IO.Path.GetDirectoryName(relativePath)?.Replace('\\', '/');
                if (directory == null)
                {
                    Log.Add($"Warning: File '{relativePath}', folder doesn't exist on drive");
                    continue;
                }

                if (!FolderExists(directory))
                {
                    Log.Add($"Warning: File '{relativePath}', folder doesn't exist in DNN DB");
                    continue;
                }

                var folderInfo = GetOqtFolderByName(directory);

                if (!FileExists(folderInfo, fileName))
                {
                    Log.Add($"Warning: File '{relativePath}', file doesn't exist in DNN DB");
                    continue;
                }

                var fileInfo = GetFile(folderInfo, fileName);
                fileIdMap.Add(fileId, fileInfo.FileId);
                Log.Add($"Map: {fileId} will be {fileInfo.FileId} ({relativePath})");
            }

            wrapLog(null);
        }

        public override void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog)
        {
            var wrapLog = Log.Call($"folders and paths: {foldersAndPath.Count}");

            foreach (var folder in foldersAndPath)
                try
                {
                    if (string.IsNullOrEmpty(folder.Value))
                    {
                        Log.Add($"{folder.Key} / {folder.Value} is empty");
                        continue;
                    }
                    var directory = IO.Path.GetDirectoryName(folder.Value);
                    if (directory == null)
                    {
                        Log.Add($"Parent folder of folder {folder.Value} doesn't exist");
                        continue;
                    }
                    // if not exist, create - important because we need for metadata assignment
                    var exists = FolderExists(directory);
                    var folderInfo = !exists
                        ? AddFolder(directory)
                        : GetOqtFolderByName(directory);

                    folderIdCorrectionList.Add(folder.Key, folderInfo.FolderId);
                    Log.Add(
                        $"Folder original #{folder.Key}/{folder.Value} - directory exists:{exists} placed in folder #{folderInfo.FolderId}");
                }
                catch (Exception)
                {
                    var msg =
                        $"Had a problem with folder of '{folder.Key}' path '{folder.Value}' - you'll have to figure out yourself if this is a problem";
                    Log.Add(msg);
                    importLog.Add(new Message(msg, Message.MessageTypes.Warning));
                }

            wrapLog($"done - final count {folderIdCorrectionList.Count}");
        }

        /*** TODO: refactor code clones ***/
        private File Add(Folder parent, IO.Stream body, string fileName, OqtSite oqtSite)
        {
            var callLog = Log.Call<File>($"Add {fileName}, folderId:{parent.FolderId}, siteId {oqtSite.Id}");

            var fullContentPath = IO.Path.Combine(_oqtServerPaths.FullContentPath(oqtSite.ContentPath), parent.Path);
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
            var oqtFile = _oqtFileRepository.AddFile(oqtFileData);
            return callLog("ok", oqtFile);
        }

        private bool FolderExists(string path) => GetOqtFolderByName(path) != null;

        private bool FileExists(Folder folderInfo, string fileName) => GetFile(folderInfo, fileName) != null;

        private File GetFile(Folder folderInfo, string fileName)
            => _oqtFileRepository.GetFiles(folderInfo.FolderId)
                .FirstOrDefault(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

        private Folder GetOqtFolderByName(string path) => _oqtFolderRepository.GetFolder(Site.Id, EnsureOqtaneFolderFormat(path));

        private Folder AddFolder(string path)
        {
            path = EnsureOqtaneFolderFormat(path);
            var callLog = Log.Call<Folder>(path);

            if (FolderExists(path)) return callLog("error, missing folder", null);

            try
            {
                // find parent
                var pathWithPretendFileName = path.TrimEnd('\\');
                var parent = IO.Path.GetDirectoryName(pathWithPretendFileName) + IO.Path.DirectorySeparatorChar;
                var subfolder = IO.Path.GetFileName(pathWithPretendFileName);
                var parentFolder = GetOqtFolderByName(parent) ?? GetOqtFolderByName("");

                // Create the new virtual folder
                var newFolder = CreateVirtualFolder(parentFolder, path, subfolder);
                return callLog("ok", newFolder);
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
                Log.Add("error in SQL, probably folder already exists");
            }
            catch (NullReferenceException)
            {
                // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
                Log.Add("error, probably folder already exists");
            }

            return callLog("?", null);
        }

        // ensure backslash on the end of path, but not on the start
        private string EnsureOqtaneFolderFormat(string path)
            => (path.Trim().Backslash().TrimEnd('\\') + '\\').TrimStart('\\');

        private Folder CreateVirtualFolder(Folder parentFolder, string path, string folder)
        {
            var newVirtualFolder = new Folder
            {
                SiteId = Site.Id,
                ParentId = parentFolder.FolderId,
                Name = folder,
                Path = path,
                Order = 1,
                IsSystem = true,
                Permissions = new List<Permission>
                {
                    new Permission(PermissionNames.View, RoleNames.Everyone, true),
                }.EncodePermissions()
            };
            _oqtFolderRepository.AddFolder(newVirtualFolder);
            return newVirtualFolder;
        }


    }
}
