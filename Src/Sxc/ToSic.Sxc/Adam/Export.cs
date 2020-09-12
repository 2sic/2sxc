using System.Collections.Generic;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Export helper
    /// provides a list of all files / folders in ADAM for export
    /// </summary>
    public class Export<TFolderId, TFileId>
    {
        private readonly IFolder _root;
        // todo #adamid - should use TFile/TFolder
        private readonly List<int> _fileIds = new List<int>();
        private readonly List<int> _folderIds = new List<int>();

        private readonly IAdamFileSystem<TFolderId, TFileId> _envFs;

        public Export(AdamAppContext<TFolderId, TFileId> adm)
        {
            _root = adm.RootFolder;
            _envFs = adm.AdamFs;
        }

        public List<int> AppFiles
        {
            get
            {
                if (_fileIds.Count == 0)
                    AddFolder(_root);
                return _fileIds;
            }
        }

        public List<int> AppFolders
        {
            get
            {
                if (_folderIds.Count == 0)
                    AddFolder(_root);
                return _folderIds;
            }
            
        } 
        private void AddFolder(IFolder folder)
        {
            _folderIds.Add(folder.Id);  // track of the folder
            AddFilesInFolder(folder);   // keep track of the files

            foreach (var f in _envFs.GetFolders(folder))   // then add subfolders
                AddFolder(f);
        }

        private void AddFilesInFolder(IFolder folder) 
            => _envFs.GetFiles(folder).ForEach(f => _fileIds.Add(f.Id));
    }
}