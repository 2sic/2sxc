using System.Collections.Generic;
using ToSic.Eav.Apps.Assets;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam
{
    public class Export
    {
        private readonly Folder _root;
        private readonly List<int> _fileIds = new List<int>();
        private readonly List<int> _folderIds = new List<int>();

        private readonly IEnvironmentFileSystem _envFs;

        public Export(AdamManager adm)
        {
            _root = adm.Root;
            _envFs = adm.EnvironmentFs;
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
        private void AddFolder(Folder fldr)
        {
            _folderIds.Add(fldr.Id);  // track of the folder
            AddFilesInFolder(fldr);         // keep track of the files

            foreach (var f in _envFs.GetFolders(fldr.Id, null))   // then add subfolders
                AddFolder(f);
        }

        private void AddFilesInFolder(Folder fldr)
        {
            foreach (var f in _envFs.GetFiles(fldr.Id, null))
                _fileIds.Add(f.Id);
        }

        
    }
}