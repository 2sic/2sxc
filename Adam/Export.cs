using System.Collections.Generic;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class Export
    {
        IFolderInfo _root;
        private AdamManager _manager;
        List<int> _fileIds = new List<int>();
        List<int> _folderIds = new List<int>();
        List<int> _emptyFolderIds = new List<int>();

        private IFolderManager _fldm = FolderManager.Instance;

        public Export(AdamManager adm)
        {
            _manager = adm;
            _root = _manager.Root;
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
        private void AddFolder(IFolderInfo fldr)
        {
            _folderIds.Add(fldr.FolderID);  // track of the folder
            AddFilesInFolder(fldr);         // keep track of the files

            foreach (var f in _fldm.GetFolders(fldr))   // then add subfolders
                AddFolder(f);
        }

        private void AddFilesInFolder(IFolderInfo fldr)
        {
            foreach (var f in _fldm.GetFiles(fldr))
                _fileIds.Add(f.FileId);
        }

        
    }
}