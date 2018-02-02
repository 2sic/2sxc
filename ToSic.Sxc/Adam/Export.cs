using System.Collections.Generic;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class Export
    {
        private readonly FolderInfo _root;
        private readonly List<int> _fileIds = new List<int>();
        private readonly List<int> _folderIds = new List<int>();

        private readonly IFolderManager _fldm = FolderManager.Instance;
        private readonly DnnFileSystem _dnnfs = new DnnFileSystem();

        public Export(AdamManager adm)
        {
            _root = adm.Root;
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
        private void AddFolder(FolderInfo fldr)
        {
            _folderIds.Add(fldr.FolderID);  // track of the folder
            AddFilesInFolder(fldr);         // keep track of the files

            foreach (var f in _dnnfs.GetFolders(fldr.FolderID, null))   // then add subfolders
                AddFolder(f);
        }

        private void AddFilesInFolder(FolderInfo fldr)
        {
            foreach (var f in _dnnfs.GetFiles(fldr.FolderID, null))
                _fileIds.Add(f.FileId);
        }

        
    }
}