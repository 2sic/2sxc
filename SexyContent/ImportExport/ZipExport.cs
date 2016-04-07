using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
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
        //private readonly SxcInstance _sexy;
        private readonly App App;
        private string _sexycontentContentgroupName = "2SexyContent-ContentGroup";
        private string _blankGuid = Guid.Empty.ToString();// "00000000-0000-0000-0000-000000000000";
        private string _zipFolderForPortalFiles = "PortalFiles";
        private string _zipFolderForAppStuff = "2sexy";
        private string _AppXmlFileName = "App.xml";

        public FileManager FileManager;

        public ZipExport(int zoneId, int appId)
        {
            _appId = appId;
            _zoneId = zoneId;
            //_sexy = new SxcInstance(_zoneId, _appId);
            App = new App(zoneId, appId, PortalSettings.Current);
            FileManager = new FileManager(App.PhysicalPath);
        }

        public MemoryStream ExportApp(bool includeContentGroups = false, bool resetAppGuid = false)
        {
            // Get Export XML
            var attributeSets = App.TemplateManager.GetAvailableContentTypes(true);
            attributeSets = attributeSets.Where(a => !a.ConfigurationIsOmnipresent);

            var attributeSetIds = attributeSets.Select(p => p.AttributeSetId.ToString()).ToArray();
			var entities = DataSource.GetInitialDataSource(_zoneId, _appId).Out["Default"].List.Where(e => e.Value.AssignmentObjectTypeId != ContentTypeHelpers.AssignmentObjectTypeIDSexyContentTemplate
				&& e.Value.AssignmentObjectTypeId != Constants.AssignmentObjectTypeIdFieldProperties).ToList();

	        if (!includeContentGroups)
	            entities = entities.Where(p => p.Value.Type.StaticName != _sexycontentContentgroupName).ToList();

            var entityIds = entities
                .Select(e => e.Value.EntityId.ToString()).ToArray();

            var messages = new List<ExportImportMessage>();
            var xmlExport = new XmlExporter(_zoneId, _appId, true,attributeSetIds, entityIds);


            #region reset App Guid if necessary
            if (resetAppGuid)
            {
                var root = xmlExport.ExportXDocument;//.Root;
                var appGuid = root.XPathSelectElement("/SexyContent/Header/App").Attribute("Guid");
                appGuid.Value = _blankGuid;
            }
            #endregion

            string xml = xmlExport.GenerateNiceXml();

            #region Copy needed files to temporary directory

            var randomShortFolderName = Guid.NewGuid().ToString().Substring(0, 4);
            var temporaryDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine(Settings.TemporaryDirectory, randomShortFolderName));

            if (!Directory.Exists(temporaryDirectoryPath))
                Directory.CreateDirectory(temporaryDirectoryPath);

            AddInstructionsToPackageFolder(temporaryDirectoryPath);

            var tempDirectory = new DirectoryInfo(temporaryDirectoryPath);
            var appDirectory = tempDirectory.CreateSubdirectory("Apps/" + App.Folder + "/");
            
            var sexyDirectory = appDirectory.CreateSubdirectory(_zipFolderForAppStuff);
            
            var portalFilesDirectory = appDirectory.CreateSubdirectory(_zipFolderForPortalFiles);

            // Copy app folder
            if (Directory.Exists(App.PhysicalPath))
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
            
            // Save export xml
            File.AppendAllText(Path.Combine(appDirectory.FullName, _AppXmlFileName), xml);

            #endregion

            // Zip directory and return as stream
            var stream = new MemoryStream();
            var zipStream = new ZipOutputStream(stream);
            zipStream.SetLevel(6);
            ZipFolder(tempDirectory.FullName + "\\", tempDirectory.FullName + "\\", zipStream);
            zipStream.Finish();

            tempDirectory.Delete(true);

            return stream;
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

        public static void ZipFolder(string RootFolder, string CurrentFolder, ZipOutputStream zStream)
        {

            var SubFolders = Directory.GetDirectories(CurrentFolder);
            foreach (var Folder in SubFolders)
                ZipFolder(RootFolder, Folder, zStream);

            var relativePath = CurrentFolder.Substring(RootFolder.Length) + "\\";

            if (relativePath.Length > 1)
            {
                var dirEntry = new ZipEntry(relativePath);
                dirEntry.DateTime = DateTime.Now;
            }
            foreach (var file in Directory.GetFiles(CurrentFolder))
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

        //public MemoryStream GetZipStream()
        //{
        //    var stream = new MemoryStream();
        //    var zipStream = new ZipOutputStream(stream);
        //    zipStream.SetLevel(0);

        //    foreach (string file in Files)
        //    {

        //        string entryName = (file.Substring(file.LastIndexOf('/')));
        //        entryName = ZipEntry.CleanName(entryName);
        //        var entry = new ZipEntry(entryName) { DateTime = DateTime.Now };
        //        // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
        //        // you need to do one of the following: Specify UseZip64.Off, or set the Size.
        //        // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
        //        // but the zip will be in Zip64 format which not all utilities can understand.
        //        // ZipStream.UseZip64 = UseZip64.Off;
        //        // Entry.Size = FileInfo.Stream.Length;

        //        zipStream.PutNextEntry(entry);
        //        FileInfo.Stream.CopyTo(zipStream);
        //        zipStream.CloseEntry();
        //        FileInfo.Stream.Close();
        //    }

        //    //Response.AddHeader("content-disposition", "attachment;filename=Images_" + Product.ArtNr + ".zip");
        //    //Response.ContentType = ContentType;
        //    //ZipStream.IsStreamOwner = true;
        //    zipStream.Close();
        //    //Response.Flush();
        //    //Response.Close();

        //    return stream;
        //}
    }
}