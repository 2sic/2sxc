using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.XPath;
using DotNetNuke.Entities.Portals;
using ICSharpCode.SharpZipLib.Zip;
using ToSic.Eav;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ImportExport
{
    public class ZipExport
    {
        private readonly int _appId;
        private readonly int _zoneId;
        private readonly App _app;
        private string _sexycontentContentgroupName = "2SexyContent-ContentGroup";
        private string _sourceControlDataFolder = ".data";
        private string _sourceControlDataFile = "app.xml"; // lower case
        private readonly string _blankGuid = Guid.Empty.ToString();
        private string _zipFolderForPortalFiles = "PortalFiles";
        private string _zipFolderForAppStuff = "2sexy";
        private string _AppXmlFileName = "App.xml";

        public FileManager FileManager;

        public ZipExport(int zoneId, int appId)
        {
            _appId = appId;
            _zoneId = zoneId;
            //_sexy = new SxcInstance(_zoneId, _appId);
            _app = new App(zoneId, appId, PortalSettings.Current);
            FileManager = new FileManager(_app.PhysicalPath);
        }

        public void ExportForSourceControl(bool includeContentGroups = false, bool resetAppGuid = false)
        {
            var path = _app.PhysicalPath + "\\" + _sourceControlDataFolder;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // generate the XML & save
            var xmlExport = GenerateExportXml(includeContentGroups, resetAppGuid);
            string xml = xmlExport.GenerateNiceXml();
            File.WriteAllText(Path.Combine(path, _sourceControlDataFile), xml);

        }

        public MemoryStream ExportApp(bool includeContentGroups = false, bool resetAppGuid = false)
        {
            // generate the XML
            var xmlExport = GenerateExportXml(includeContentGroups, resetAppGuid);

            #region Copy needed files to temporary directory

            var messages = new List<ExportImportMessage>();
            var randomShortFolderName = Guid.NewGuid().ToString().Substring(0, 4);

            var temporaryDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine(Settings.TemporaryDirectory, randomShortFolderName));

            if (!Directory.Exists(temporaryDirectoryPath))
                Directory.CreateDirectory(temporaryDirectoryPath);

            AddInstructionsToPackageFolder(temporaryDirectoryPath);

            var tempDirectory = new DirectoryInfo(temporaryDirectoryPath);
            var appDirectory = tempDirectory.CreateSubdirectory("Apps/" + _app.Folder + "/");
            
            var sexyDirectory = appDirectory.CreateSubdirectory(_zipFolderForAppStuff);
            
            var portalFilesDirectory = appDirectory.CreateSubdirectory(_zipFolderForPortalFiles);

            // Copy app folder
            if (Directory.Exists(_app.PhysicalPath))
            {
                FileManager.CopyAllFiles(sexyDirectory.FullName, false, messages);
            }

            // Copy PortalFiles
            foreach (var file in xmlExport.ReferencedFiles)
            {
                var portalFilePath = Path.Combine(portalFilesDirectory.FullName, Path.GetDirectoryName(file.RelativePath.Replace('/','\\')));

                if (!Directory.Exists(portalFilePath))
                    Directory.CreateDirectory(portalFilePath);

                if (File.Exists(file.PhysicalPath))
                {
                    var fullPath = Path.Combine(portalFilesDirectory.FullName, file.RelativePath.Replace('/', '\\'));
                    try
                    {
                        File.Copy(file.PhysicalPath, fullPath);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error on " + fullPath + " (" + fullPath.Length + ")", e);
                    }
                }
            }
            #endregion


            // Save export xml
            string xml = xmlExport.GenerateNiceXml();
            File.AppendAllText(Path.Combine(appDirectory.FullName, _AppXmlFileName), xml);


            // Zip directory and return as stream
            var stream = new MemoryStream();
            var zipStream = new ZipOutputStream(stream);
            zipStream.SetLevel(6);
            ZipFolder(tempDirectory.FullName + "\\", tempDirectory.FullName + "\\", zipStream);
            zipStream.Finish();

            tempDirectory.Delete(true);

            return stream;
        }

        private XmlExporter GenerateExportXml(bool includeContentGroups, bool resetAppGuid)
        {
// Get Export XML
            var attributeSets = _app.TemplateManager.GetAvailableContentTypes(true);
            attributeSets = attributeSets.Where(a => !a.ConfigurationIsOmnipresent);

            var attributeSetIds = attributeSets.Select(p => p.AttributeSetId.ToString()).ToArray();
            var entities =
                DataSource.GetInitialDataSource(_zoneId, _appId).Out["Default"].List.Where(
                    e => e.Value.AssignmentObjectTypeId != ContentTypeHelpers.AssignmentObjectTypeIDSexyContentTemplate
                         && e.Value.AssignmentObjectTypeId != Constants.AssignmentObjectTypeIdFieldProperties).ToList();

            if (!includeContentGroups)
                entities = entities.Where(p => p.Value.Type.StaticName != _sexycontentContentgroupName).ToList();

            var entityIds = entities
                .Select(e => e.Value.EntityId.ToString()).ToArray();

            var xmlExport = new XmlExporter(_zoneId, _appId, true, attributeSetIds, entityIds);

            #region reset App Guid if necessary

            if (resetAppGuid)
            {
                var root = xmlExport.ExportXDocument; //.Root;
                var appGuid = root.XPathSelectElement("/SexyContent/Header/App").Attribute("Guid");
                appGuid.Value = _blankGuid;
            }
            return xmlExport;
            #endregion
        }

        /// <summary>
        /// This adds various files to an app-package, so anybody who gets such a package
        /// is informed as to what they must do with it.
        /// </summary>
        /// <param name="targetPath"></param>
        private void AddInstructionsToPackageFolder(string targetPath)
        {
            var srcPath = HttpContext.Current.Server.MapPath(Path.Combine(Settings.ToSexyDirectory, "SexyContent\\ImportExport\\Instructions"));

            foreach (var file in Directory.GetFiles(srcPath))
                File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)));


        }

        public static void ZipFolder(string rootFolder, string currentFolder, ZipOutputStream zStream)
        {

            var SubFolders = Directory.GetDirectories(currentFolder);
            foreach (var Folder in SubFolders)
                ZipFolder(rootFolder, Folder, zStream);

            var relativePath = currentFolder.Substring(rootFolder.Length) + "\\";

            if (relativePath.Length > 1)
            {
                var dirEntry = new ZipEntry(relativePath);
                dirEntry.DateTime = DateTime.Now;
            }
            foreach (var file in Directory.GetFiles(currentFolder))
            {
                AddFileToZip(zStream, relativePath, file);
            }
        }



        private static void AddFileToZip(ZipOutputStream zStream, string relativePath, string file)
        {
            var buffer = new byte[4096];
            var fileRelativePath = (relativePath.Length > 1 ? relativePath : string.Empty) + Path.GetFileName(file);
            var entry = new ZipEntry(fileRelativePath);
            entry.DateTime = DateTime.Now;
            zStream.PutNextEntry(entry);
            using (var fs = File.OpenRead(file))
            {
                int sourceBytes;
                do
                {
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    zStream.Write(buffer, 0, sourceBytes);

                } while (sourceBytes > 0);
            }
        }

    }
}