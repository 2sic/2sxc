using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToSic.SexyContent.ImportExport
{
    public class FileManager
    {

        public FileManager(string sourceFld)
        {
            _sourceFolder = sourceFld;

        }

        /// <summary>
        /// Folder-names of folders which won't be exported or imported
        /// </summary>

        private readonly string _sourceFolder;

        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder (directly on the file system)
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="overwriteFiles"></param>
        /// <param name="messages"></param>
        public void CopyAllFiles(string destinationFolder, bool overwriteFiles, List<ExportImportMessage> messages)
        {
            var filteredFiles = AllTransferableFiles;

            foreach (var file in filteredFiles)
            {
                var relativeFilePath = file.Replace(_sourceFolder, "");
                var destinationFilePath = String.Format("{0}{1}{2}",
                destinationFolder, Path.DirectorySeparatorChar, relativeFilePath);
                
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));

                if (!File.Exists(destinationFilePath))
                    File.Copy(file, destinationFilePath, overwriteFiles);
                else
                    messages.Add(new ExportImportMessage("File '" + Path.GetFileName(destinationFilePath) + "' not copied because it already exists", ExportImportMessage.MessageTypes.Warning));
            }
        }

        private IEnumerable<string> _allTransferableFiles; 
        /// <summary>
        /// Gets all files from a folder and subfolder, which fit the import/export filter criteria
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AllTransferableFiles
        {
            get
            {
                if (_allTransferableFiles == null)
                {
                    // add folder slashes to ensure the term is part of a folder, not within a file-name
                    var excFolderFilter = Constants.ExcludeFolders.Select(f => "\\" + f + "\\").ToArray();

                    _allTransferableFiles = from f in AllFiles
                        where !excFolderFilter.Any(ex => f.ToLowerInvariant().Contains(ex))
                        select f;
                }
                return _allTransferableFiles;
            }
        }

        private IEnumerable<string> _allFiles; 

        /// <summary>
        /// Get all files from a folder, not caring if it will be exported or not
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AllFiles => _allFiles ??
                                               (_allFiles = Directory.EnumerateFiles(_sourceFolder, "*.*", SearchOption.AllDirectories));
    }
}