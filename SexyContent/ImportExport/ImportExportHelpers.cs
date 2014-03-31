using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.ImportExport
{
    public class ImportExportHelpers
    {
        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder (directly on the file system)
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="overwriteFiles"></param>
        /// <param name="messages"></param>
        public static void CopyAllFiles(string sourceFolder, string destinationFolder, Boolean overwriteFiles, List<ExportImportMessage> messages)
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
    }
}