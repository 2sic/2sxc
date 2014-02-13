using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using ICSharpCode.SharpZipLib.Zip;

namespace ToSic.SexyContent.ImportExport
{
    public class ZipImport
    {

        /// <summary>
        /// Imports a ZIP file (from stream)
        /// </summary>
        /// <param name="zipStream"></param>
        /// <param name="server"></param>
        /// <param name="portalSettings"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ImportZip(Stream zipStream, HttpServerUtility server, PortalSettings portalSettings, List<ExportImportMessage> messages)
        {
            if (messages == null)
                messages = new List<ExportImportMessage>();

            var temporaryDirectory = server.MapPath(Path.Combine(SexyContent.TemporaryDirectory, System.Guid.NewGuid().ToString()));
            var success = true;

            try
            {
                if (!Directory.Exists(temporaryDirectory))
                    Directory.CreateDirectory(temporaryDirectory);

                // Extract ZIP archive to the temporary folder
                ExtractZipFile(zipStream, temporaryDirectory);

                string currentWorkingDir = temporaryDirectory;
                string[] baseDirectories = Directory.GetDirectories(currentWorkingDir);

                // Loop through each root-folder. For now only contains the "Apps" folder.
                foreach (var directoryPath in baseDirectories)
                {
                    switch (Path.GetFileName(directoryPath))
                    {
                        // Handle the App folder
                        case "Apps":
                            currentWorkingDir = Path.Combine(currentWorkingDir, "Apps");

                            // Loop through each app directory
                            foreach (var appDirectory in Directory.GetDirectories(currentWorkingDir))
                            {
                                
                                // Copy all files in 2sexy folder to (portal file system) 2sexy folder
                                string templateRoot = server.MapPath(SexyContent.GetTemplatePathRoot(SexyContent.TemplateLocations.PortalFileSystem));
                                string appTemplateRoot = Path.Combine(appDirectory, "2sexy");
                                if (Directory.Exists(appTemplateRoot))
                                    CopyAllFiles(appTemplateRoot, templateRoot, false, messages);

                                // Handle PortalFiles folder
                                string portalTempRoot = Path.Combine(appDirectory, "PortalFiles");
                                if (Directory.Exists(portalTempRoot))
                                    CopyAllFilesDnnPortal(portalTempRoot, "", false, messages);


                                // Import each XML file which is in the current App folder
                                foreach (string xmlFileName in Directory.GetFiles(appDirectory, "*.xml"))
                                {
                                    var fileContents = File.ReadAllText(Path.Combine(appDirectory, xmlFileName));
                                    var import = new XmlImport();
                                    var xmlImportSuccess = import.ImportXml(fileContents);
                                    messages.AddRange(import.ImportLog);
                                }
                            }

                            // Reset CurrentWorkingDir
                            currentWorkingDir = temporaryDirectory;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // Add error message and return false
                messages.Add(new ExportImportMessage("Could not import the package. " + e.Message, ExportImportMessage.MessageTypes.Error));
                Exceptions.LogException(e);
                success = false;
            }
            finally
            {
                // Finally delete the temporary directory
                Directory.Delete(temporaryDirectory, true);
            }

            return success;
        }

        #region Zip Import Helpers

        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder">The portal-relative path where the files should be copied to</param>
        /// <param name="overwriteFiles"></param>
        /// <param name="messages"></param>
        /// <param name="fileIdMapping">The fileIdMapping is needed to re-assign the existing "File:" parameters while importing the content</param>
        private void CopyAllFilesDnnPortal(string sourceFolder, string destinationFolder, Boolean overwriteFiles, List<ExportImportMessage> messages)
        {
            var files = Directory.GetFiles(sourceFolder, "*.*");

            var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;
            var portalId = PortalSettings.Current.PortalId;

            if (!folderManager.FolderExists(portalId, destinationFolder))
                folderManager.AddFolder(portalId, destinationFolder);
            IFolderInfo folderInfo = folderManager.GetFolder(portalId, destinationFolder);

            foreach (var sourceFilePath in files)
            {
                var destinationFileName = Path.GetFileName(sourceFilePath);

                if (!fileManager.FileExists(folderInfo, destinationFileName))
                {
                    try
                    {
                        using (var stream = File.OpenRead(sourceFilePath))
                            fileManager.AddFile(folderInfo, destinationFileName, stream, false);
                    }
                    catch (InvalidFileExtensionException e)
                    {
                        messages.Add(new ExportImportMessage("File '" + destinationFileName + "' not copied because the file extension is not allowed.", ExportImportMessage.MessageTypes.Error));
                        Exceptions.LogException(e);
                    }
                }
                else
                    messages.Add(new ExportImportMessage("File '" + destinationFileName + "' not copied because it already exists", ExportImportMessage.MessageTypes.Warning));
            }

            // Call the method recursively to handle subdirectories
            foreach (var sourceFolderPath in Directory.GetDirectories(sourceFolder))
            {
                var newDestinationFolder = Path.Combine(destinationFolder, sourceFolderPath.Replace(sourceFolder, "").TrimStart('\\')).Replace('\\', '/');
                CopyAllFilesDnnPortal(sourceFolderPath, newDestinationFolder, overwriteFiles, messages);
            }
        }

        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder (directly on the file system)
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="overwriteFiles"></param>
        /// <param name="messages"></param>
        private void CopyAllFiles(string sourceFolder, string destinationFolder, Boolean overwriteFiles, List<ExportImportMessage> messages)
        {
            var FileList = from f in Directory.GetFiles(sourceFolder, "*.*", SearchOption.AllDirectories)
                           select f;

            foreach (string file in FileList)
            {
                string relativeFilePath = file.Replace(sourceFolder, "");
                string destinationFilePath = String.Format("{0}{1}{2}",
                destinationFolder, Path.DirectorySeparatorChar, relativeFilePath);

                if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                }

                if (!File.Exists(destinationFilePath))
                    File.Copy(file, destinationFilePath, overwriteFiles);
                else
                    messages.Add(new ExportImportMessage("File '" + Path.GetFileName(destinationFilePath) + "' not copied because it already exists", ExportImportMessage.MessageTypes.Warning));
            }
        }

        /// <summary>
        /// Extracts a Zip (as Stream) to the given OutFolder directory.
        /// </summary>
        /// <param name="zipStream"></param>
        /// <param name="outFolder"></param>
        private void ExtractZipFile(Stream zipStream, string outFolder)
        {
            var file = new ZipFile(zipStream);

            try
            {
                foreach (ZipEntry entry in file)
                {
                    if (entry.IsDirectory)
                        continue;
                    var fileName = entry.Name;

                    var entryStream = file.GetInputStream(entry);

                    var fullPath = Path.Combine(outFolder, fileName);
                    var directoryName = Path.GetDirectoryName(fullPath);
                    if (!String.IsNullOrEmpty(directoryName))
                        Directory.CreateDirectory(directoryName);

                    // Unzip File in buffered chunks
                    using (var streamWriter = File.Create(fullPath))
                    {
                        entryStream.CopyTo(streamWriter, 4096);
                    }
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        #endregion
    }
}