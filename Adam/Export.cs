using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class Export
    {
        IFolderInfo _root;
        private AdamManager _manager;
        List<int> _fileIds = new List<int>();

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

        private void AddFolder(IFolderInfo fldr)
        {
            AddFilesInFolder(fldr);

            foreach (var f in _fldm.GetFolders(fldr))
                AddFolder(f);
        }

        private void AddFilesInFolder(IFolderInfo fldr)
        {
            foreach (var f in _fldm.GetFiles(fldr))
                _fileIds.Add(f.FileId);
        }

        
    }
}