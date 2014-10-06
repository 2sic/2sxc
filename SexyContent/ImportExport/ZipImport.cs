using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using ICSharpCode.SharpZipLib.Zip;

namespace ToSic.SexyContent.ImportExport
{
    public class ZipImport
    {
        private int? _appId;
        private int _zoneId;
        private bool _allowRazor;
        
        public ZipImport(int zoneId, int? appId, bool allowRazor)
        {
            _appId = appId;
            _zoneId = zoneId;
            _allowRazor = allowRazor;
        }

        public bool ImportApp(Stream zipStream, HttpServerUtility server, PortalSettings portalSettings, List<ExportImportMessage> messages)
        {
            return ImportZip(zipStream, server, portalSettings, messages, true);
        }

        /// <summary>
        /// Imports a ZIP file (from stream)
        /// </summary>
        /// <param name="zipStream"></param>
        /// <param name="server"></param>
        /// <param name="portalSettings"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ImportZip(Stream zipStream, HttpServerUtility server, PortalSettings portalSettings, List<ExportImportMessage> messages, bool isAppImport)
        {
            if(!isAppImport && !_appId.HasValue)
                throw new Exception("Could not import zip: No valid app id");

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

                                var appId = new int?();
                                var xmlSearchPattern = isAppImport ? "App.xml" : "*.xml";

                                // Stores the number of the current xml file to process
                                var xmlIndex = 0;

                                // Import XML file(s)
                                foreach (var xmlFileName in Directory.GetFiles(appDirectory, "*.xml"))
                                {
                                    var fileContents = File.ReadAllText(Path.Combine(appDirectory, xmlFileName));
                                    var import = new XmlImport();

                                    if (isAppImport)
                                    {
                                        if (!import.IsCompatible(_zoneId, fileContents))
                                            throw new Exception("The " + (isAppImport ? "app" : "package") + " is not compatible with this version of 2sxc.");

                                        var folder =
                                            XDocument.Parse(fileContents).Element("SexyContent")
                                                .Element("Entities").Elements("Entity").Single(e =>e.Attribute("AttributeSetStaticName").Value =="2SexyContent-App")
                                                .Elements("Value").First(v => v.Attribute("Key").Value == "Folder").Attribute("Value").Value;
                                        var appPath = System.IO.Path.Combine(SexyContent.AppBasePath(PortalSettings.Current), folder);

                                        // Do not import (throw error) if the app directory already exists
                                        if(Directory.Exists(HttpContext.Current.Server.MapPath(appPath)))
                                        {
                                            throw new Exception("The app could not be installed because the app-folder '" + appPath + "' already exists. Please remove or rename the folder and install the app again.");
                                        }

                                        if (xmlIndex == 0)
                                        {
                                            // Handle PortalFiles folder
                                            string portalTempRoot = Path.Combine(appDirectory, "PortalFiles");
                                            if (Directory.Exists(portalTempRoot))
                                                CopyAllFilesDnnPortal(portalTempRoot, "", false, messages);
                                        }

                                        import.ImportApp(_zoneId, fileContents, out appId);
                                    }
                                    else
                                    {
                                        appId = _appId.Value;
                                        if (xmlIndex == 0 && import.IsCompatible(_zoneId, fileContents))
                                        {
                                            // Handle PortalFiles folder
                                            string portalTempRoot = Path.Combine(appDirectory, "PortalFiles");
                                            if (Directory.Exists(portalTempRoot))
                                                CopyAllFilesDnnPortal(portalTempRoot, "", false, messages);
                                        }

                                        import.ImportXml(_zoneId, appId.Value, fileContents);
                                    }

                                    
                                    messages.AddRange(import.ImportLog);

                                    xmlIndex++;
                                }

                                var sexy = new SexyContent(_zoneId, appId.Value);

                                // Copy all files in 2sexy folder to (portal file system) 2sexy folder
                                string templateRoot = server.MapPath(SexyContent.GetTemplatePathRoot(SexyContent.TemplateLocations.PortalFileSystem, sexy.App));
                                string appTemplateRoot = Path.Combine(appDirectory, "2sexy");
                                if (Directory.Exists(appTemplateRoot))
                                    ImportExportHelpers.CopyAllFiles(appTemplateRoot, templateRoot, false, messages);

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
                messages.Add(new ExportImportMessage("Could not import the " + (isAppImport ? "app" : "package") + ": " + e.Message, ExportImportMessage.MessageTypes.Error));
                Exceptions.LogException(e);
                success = false;
            }
            finally
            {
                try
                {
                    // Finally delete the temporary directory
                    Directory.Delete(temporaryDirectory, true);
                }
                catch(IOException e)
                {
                    // The folder itself or files inside may be used by other processes.
                    // Deleting the folder recursively will fail in such cases
                    // If deleting is not possible, just leave the temporary folder as it is
                }
            }

            return success;
        }

        public bool ImportZipFromUrl(string packageUrl, List<ExportImportMessage> messages, bool isAppImport)
        {
            var tempDirectory = new DirectoryInfo(HttpContext.Current.Server.MapPath(SexyContent.TemporaryDirectory));
            if (!tempDirectory.Exists)
                Directory.CreateDirectory(tempDirectory.FullName);

            var destinationPath = Path.Combine(tempDirectory.FullName, Path.GetRandomFileName() + ".zip");
            var client = new WebClient();
            var success = false;

            try
            {
                client.DownloadFile(packageUrl, destinationPath);
            }
            catch(WebException e)
            {
                throw new Exception("Could not download app package from '" + packageUrl + "'.", e);
            }

            using (var file = File.OpenRead(destinationPath))
                success = ImportZip(file, HttpContext.Current.Server, PortalSettings.Current, messages, isAppImport);

            File.Delete(destinationPath);

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