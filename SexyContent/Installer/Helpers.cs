using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Installer
{
    internal class Helpers
    {
        /// <summary>
        /// Copy a Directory recursive
        /// </summary>
        /// <remarks>Source: http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx </remarks>
        internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        internal static void ImportXmlSchemaOfVersion(string version, bool leaveOriginalsUntouched, Log parentLog = null)
        {
            //var userName = "System-ModuleUpgrade-" + version;
            var xmlToImport =
                File.ReadAllText(
                    HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/" + version + ".xml"));
            var xmlImport = new XmlImportWithFiles(parentLog, "en-US", /*userName,*/ true);
            var success = xmlImport.ImportXml(Constants.DefaultZoneId, Constants.MetaDataAppId, XDocument.Parse(xmlToImport),
                leaveOriginalsUntouched);

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Text).ToArray());
                throw new Exception("The 2sxc module upgrade to " + version + " failed: " + messages);
            }
        }

    }
}