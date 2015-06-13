using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            string[] excludeFolders = {".git", "node_modules"};
            excludeFolders = excludeFolders.Select(f => "\\" + f + "\\").ToArray();

            var allFiles = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.AllDirectories);

            var filteredFiles = from f in allFiles
                           where !excludeFolders.Any(ex => f.ToLowerInvariant().Contains(ex) )// !f.Contains("\\.git\\")
                           select f;

            foreach (var file in filteredFiles)
            {
                var relativeFilePath = file.Replace(sourceFolder, "");
                var destinationFilePath = String.Format("{0}{1}{2}",
                destinationFolder, Path.DirectorySeparatorChar, relativeFilePath);

                //if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));

                if (!File.Exists(destinationFilePath))
                    File.Copy(file, destinationFilePath, overwriteFiles);
                else
                    messages.Add(new ExportImportMessage("File '" + Path.GetFileName(destinationFilePath) + "' not copied because it already exists", ExportImportMessage.MessageTypes.Warning));
            }
        }
    }
}