using System;
using System.Collections.Generic;

namespace ToSic.Eav.ImportExport
{
    public interface IImportExportEnvironment
    {
        /// <summary>
        /// Copy all files from SourceFolder to DestinationFolder
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder">The portal-relative path where the files should be copied to</param>
        ///// <param name="overwriteFiles"></param>
        ///// <param name="messages"></param>
        ///// <param name="fileIdMapping">The fileIdMapping is needed to re-assign the existing "File:" parameters while importing the content</param>
        void TransferFilesToTennant(string sourceFolder, string destinationFolder)//, Boolean overwriteFiles, List<ExportImportMessage> messages)
            ;

        Version TennantVersion { get; }

        string ModuleVersion { get; }

        /// <summary>
        /// This is used for import-cases, where the scope
        /// is missing in the file
        /// </summary>
        string FallbackContentTypeScope { get; }
        string DefaultLanguage { get; }
        string TemplatesRoot(int zoneId, int appId);
        string TargetPath(string folder);
        void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap);
        void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<ExportImportMessage> importLog);
    }
}