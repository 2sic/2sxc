using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Entities.Portals;
using ICSharpCode.SharpZipLib.Zip;
using ToSic.Eav;

namespace ToSic.SexyContent.ImportExport
{
    public class ZipExport
    {
        private int _appId;
        private int _zoneId;
        private SexyContent _sexy;

        public ZipExport(int zoneId, int appId)
        {
            _appId = appId;
            _zoneId = zoneId;
            _sexy = new SexyContent(_zoneId, _appId);
        }

        public MemoryStream ExportApp()
        {
            // Get Export XML
            var attributeSets = _sexy.GetAvailableAttributeSets(SexyContent.AttributeSetScope).ToList();
            attributeSets.AddRange(_sexy.GetAvailableAttributeSets(SexyContent.AttributeSetScopeApps));
            attributeSets = attributeSets.Where(a => !a.UsesConfigurationOfAttributeSet.HasValue).ToList();

            // Special case: Entities of the template attributesets and field properties should not be exported here
            //var templateAttributeSets = _sexy.GetAvailableAttributeSets().Where(a => a.StaticName == SexyContent.AttributeSetStaticNameTemplateContentTypes
            //                    || a.StaticName == SexyContent.AttributeSetStaticNameTemplateMetaData
            //                    || a.StaticName.StartsWith("@"));

            // Thoughts... maybe exclude entities that have assignmentobject type "EAV Field Properties" and "2SexyContent-Template"

            var attributeSetIds = attributeSets.Select(p => p.AttributeSetID.ToString()).ToArray();
            var entities = SexyContent.GetInitialDataSource(_zoneId, _appId).Out["Default"].List;
            var entityIds = entities.Where(e => e.Value.AssignmentObjectTypeId != SexyContent.AssignmentObjectTypeIDSexyContentTemplate 
                && e.Value.AssignmentObjectTypeId != DataSource.AssignmentObjectTypeIdFieldProperties)
                .Select(e => e.Value.EntityId.ToString()).ToArray();

            var templateIds = _sexy.GetTemplates(PortalSettings.Current.PortalId).Select(p => p.TemplateID.ToString()).ToArray();
            var messages = new List<ExportImportMessage>();
            var xmlExport = new XmlExport(_zoneId, _appId, true);
            var xml = xmlExport.ExportXml(attributeSetIds, entityIds, templateIds, out messages);

            #region Copy needed files to temporary directory

            var temporaryDirectoryPath = HttpContext.Current.Server.MapPath(Path.Combine(SexyContent.TemporaryDirectory, System.Guid.NewGuid().ToString()));

            if (!Directory.Exists(temporaryDirectoryPath))
                Directory.CreateDirectory(temporaryDirectoryPath);

            var tempDirectory = new DirectoryInfo(temporaryDirectoryPath);
            var appDirectory = tempDirectory.CreateSubdirectory("Apps/" + _sexy.App.Folder + "/");
            var sexyDirectory = appDirectory.CreateSubdirectory("2sexy");
            var portalFilesDirectory = appDirectory.CreateSubdirectory("PortalFiles");

            // Copy app folder
            if (Directory.Exists(_sexy.App.PhysicalPath))
            {
                ImportExportHelpers.CopyAllFiles(_sexy.App.PhysicalPath, sexyDirectory.FullName, false, messages);
            }

            // Copy PortalFiles
            foreach (var file in xmlExport.ReferencedFiles)
            {
                var portalFilePath = Path.Combine(portalFilesDirectory.FullName, Path.GetDirectoryName(file.RelativePath.Replace('/','\\')));

                if (!Directory.Exists(portalFilePath))
                    Directory.CreateDirectory(portalFilePath);
                
                File.Copy(file.PhysicalPath, Path.Combine(portalFilesDirectory.FullName, file.RelativePath.Replace('/', '\\')));
            }
            
            // Save export xml
            File.AppendAllText(System.IO.Path.Combine(appDirectory.FullName, "App.xml"), xml);

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

        public static void ZipFolder(string RootFolder, string CurrentFolder, ZipOutputStream zStream)
        {

            string[] SubFolders = Directory.GetDirectories(CurrentFolder);
            foreach (string Folder in SubFolders)
                ZipFolder(RootFolder, Folder, zStream);

            string relativePath = CurrentFolder.Substring(RootFolder.Length) + "\\";

            if (relativePath.Length > 1)
            {
                var dirEntry = new ZipEntry(relativePath);
                dirEntry.DateTime = DateTime.Now;
            }
            foreach (string file in Directory.GetFiles(CurrentFolder))
            {
                AddFileToZip(zStream, relativePath, file);
            }
        }



        private static void AddFileToZip(ZipOutputStream zStream, string relativePath, string file)
        {
            byte[] buffer = new byte[4096];
            string fileRelativePath = (relativePath.Length > 1 ? relativePath : string.Empty) + Path.GetFileName(file);
            ZipEntry entry = new ZipEntry(fileRelativePath);
            entry.DateTime = DateTime.Now;
            zStream.PutNextEntry(entry);
            using (FileStream fs = File.OpenRead(file))
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