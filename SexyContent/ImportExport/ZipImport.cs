using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;
using ToSic.Eav;
//using ToSic.Eav.DataSources.Caches;
//using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ImportExport
{
    public class ZipImport
    {
        private int? _appId;
        private readonly int _zoneId;
        private bool _allowRazor;
        private readonly ImportExportEnvironment _environment;
        public ZipImport(ImportExportEnvironment environment, int zoneId, int? appId, bool allowRazor)
        {
            _appId = appId;
            _zoneId = zoneId;
            _allowRazor = allowRazor;
            _environment = environment;

        }

   //     public bool ImportApp(Stream zipStream, HttpServerUtility server, PortalSettings portalSettings, List<ExportImportMessage> messages)
   //     {
			//return ImportZip(zipStream, server, portalSettings, messages);
   //     }

        /// <summary>
        /// Imports a ZIP file (from stream)
        /// </summary>
        /// <param name="zipStream"></param>
        /// <param name="server"></param>
        ///// <param name="portalSettings"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ImportZip(Stream zipStream, HttpServerUtility server/*, PortalSettings portalSettings, List<ExportImportMessage> messages*/)
        {
            List<ExportImportMessage> messages = _environment.Messages;
            //if (messages == null)
            //    messages = _environment.Messages;// new List<ExportImportMessage>();

            var temporaryDirectory = server.MapPath(Path.Combine(Settings.TemporaryDirectory, Guid.NewGuid().ToString()));
            var success = true;

            try
            {
                if (!Directory.Exists(temporaryDirectory))
                    Directory.CreateDirectory(temporaryDirectory);

                // Extract ZIP archive to the temporary folder
                ExtractZipFile(zipStream, temporaryDirectory);

                var currentWorkingDir = temporaryDirectory;
                var baseDirectories = Directory.GetDirectories(currentWorkingDir);

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

                                // Stores the number of the current xml file to process
                                var xmlIndex = 0;

                                // Import XML file(s)
                                foreach (var xmlFileName in Directory.GetFiles(appDirectory, "*.xml"))
                                {
                                    var fileContents = File.ReadAllText(Path.Combine(appDirectory, xmlFileName));
	                                var doc = XDocument.Parse(fileContents);
									var import = new XmlImport(_environment.DefaultLanguage, Environment.Dnn7.UserIdentity.CurrentUserIdentityToken /*PortalSettings.Current.UserInfo.Username*/);

									if (!import.IsCompatible(doc))
										throw new Exception("The app / package is not compatible with this version of 2sxc.");

									var isAppImport = doc.Element("SexyContent").Element("Header").Elements("App").Any() && doc.Element("SexyContent").Element("Header").Element("App").Attribute("Guid").Value != "Default";

                                    if (!isAppImport && !_appId.HasValue)
                                        _appId = State.GetDefaultAppId(_zoneId);// ((BaseCache) DataSource.GetCache(_zoneId)).ZoneApps[_zoneId].DefaultAppId;

                                    if (isAppImport)
                                    {
                                        var appConfig = XDocument.Parse(fileContents).Element("SexyContent")
                                            .Element("Entities")
                                            .Elements("Entity")
                                            .Single(e => e.Attribute("AttributeSetStaticName").Value == "2SexyContent-App");

                                        #region Version Checks (new in 08.03.03)
                                        var reqVersionNode = appConfig.Elements("Value")?.FirstOrDefault(v => v.Attribute("Key").Value == "RequiredVersion")?.Attribute("Value")?.Value;
                                        var reqVersionNodeDnn = appConfig.Elements("Value")?.FirstOrDefault(v => v.Attribute("Key").Value == "RequiredDnnVersion")?.Attribute("Value")?.Value;

                                        CheckRequiredEnvironmentVersions(reqVersionNode, reqVersionNodeDnn);

                                        #endregion
                                        var folder = appConfig.Elements("Value").First(v => v.Attribute("Key").Value == "Folder").Attribute("Value").Value;

                                        //var appPath = Path.Combine(AppHelpers.AppBasePath(null), folder);

                                        // Do not import (throw error) if the app directory already exists
                                        var appPath = _environment.TargetPath(folder);
                                        if (Directory.Exists(appPath))
                                        {
                                            throw new Exception("The app could not be installed because the app-folder '" + appPath + "' already exists. Please remove or rename the folder and install the app again.");
                                        }

                                        if (xmlIndex == 0)
                                        {
                                            // Handle PortalFiles folder
                                            var portalTempRoot = Path.Combine(appDirectory, "PortalFiles");
                                            if (Directory.Exists(portalTempRoot))
                                                _environment.TransferFilesToTennant(portalTempRoot, "");//, false, messages);
                                        }

                                        import.ImportApp(_zoneId, doc, out appId);
                                    }
                                    else
                                    {
                                        appId = _appId.Value;
                                        if (xmlIndex == 0 && import.IsCompatible(doc))
                                        {
                                            // Handle PortalFiles folder
                                            var portalTempRoot = Path.Combine(appDirectory, "PortalFiles");
                                            if (Directory.Exists(portalTempRoot))
                                                _environment.TransferFilesToTennant(portalTempRoot, "");//, false, messages);
                                        }

                                        import.ImportXml(_zoneId, appId.Value, doc);
                                    }

                                    
                                    messages.AddRange(import.ImportLog);

                                    xmlIndex++;
                                }

                                //var sexy = new SxcInstance(_zoneId, appId.Value);
                                // var app = new App(_zoneId, appId.Value,  PortalSettings.Current, false);

                                // Copy all files in 2sexy folder to (portal file system) 2sexy folder
                                var templateRoot = _environment.TemplatesRoot(_zoneId, appId.Value);// server.MapPath(Internal.TemplateManager.GetTemplatePathRoot(Settings.TemplateLocations.PortalFileSystem, app));
                                var appTemplateRoot = Path.Combine(appDirectory, "2sexy");
                                if (Directory.Exists(appTemplateRoot))
                                    new FileManager(appTemplateRoot).CopyAllFiles(templateRoot, false, messages);

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
                messages.Add(new ExportImportMessage("Could not import the app / package: " + e.Message, ExportImportMessage.MessageTypes.Error));
                // Exceptions.LogException(e);
                success = false;
            }
            finally
            {
                try
                {
                    // Finally delete the temporary directory
                    Directory.Delete(temporaryDirectory, true);
                }
                catch(Exception ex) when (ex is FormatException || ex is OverflowException) 
                {
                    // The folder itself or files inside may be used by other processes.
                    // Deleting the folder recursively will fail in such cases
                    // If deleting is not possible, just leave the temporary folder as it is
                }
            }

            return success;
        }

        private void CheckRequiredEnvironmentVersions(string reqVersionNode, string reqVersionNodeDnn)
        {
            if (reqVersionNode != null)
            {
                var vSxc = Settings.Version;
                var reqSxcV = Version.Parse(reqVersionNode);
                if (reqSxcV.CompareTo(vSxc) == 1) // required is bigger
                    throw new Exception("this app requires 2sxc version " + reqVersionNode +
                                        ", installed is " + vSxc + ". cannot continue. see also 2sxc.org/en/help?tag=app");
            }

            if (reqVersionNodeDnn != null)
            {
                var vDnn = _environment.Version;
                var reqDnnV = Version.Parse(reqVersionNodeDnn);
                if (reqDnnV.CompareTo(vDnn) == 1) // required is bigger
                    throw new Exception("this app requires dnn version " + reqVersionNodeDnn +
                                        ", installed is "+vDnn +". cannot continue. see also 2sxc.org/en/help?tag=app");
            }
        }

        public bool ImportZipFromUrl(string packageUrl, bool isAppImport)
        {
            var tempDirectory = new DirectoryInfo(HttpContext.Current.Server.MapPath(Settings.TemporaryDirectory));
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
                success = ImportZip(file, HttpContext.Current.Server/*, PortalSettings.Current, _environment.Messages*/);

            File.Delete(destinationPath);

            return success;
        }

        #region Zip Import Helpers



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