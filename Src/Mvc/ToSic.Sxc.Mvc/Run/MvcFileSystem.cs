using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Sxc.Adam;
using File = ToSic.Sxc.Adam.File;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcFileSystem: IEnvironmentFileSystem
    {
        public IEnvironmentFileSystem Init(AdamAppContext adamContext)
        {
            throw new NotImplementedException();
        }

        public int MaxUploadKb()
        {
            throw new NotImplementedException();
        }

        public IFile GetFile(int fileId)
        {
            throw new NotImplementedException();
        }

        public List<File> GetFiles(int folderId)
        {
            throw new NotImplementedException();
        }

        public void Rename(IFile file, string newName)
        {
            throw new NotImplementedException();
        }

        public void Delete(IFile file)
        {
            throw new NotImplementedException();
        }

        public IFile Add(IFolder parent, Stream body, string fileName, bool ensureUniqueName)
        {
            throw new NotImplementedException();
        }

        public void AddFolder(int tenantId, string path)
        {
            throw new NotImplementedException();
        }

        public bool FolderExists(int tenantId, string path)
        {
            throw new NotImplementedException();
        }

        public Folder GetFolder(int folderId)
        {
            throw new NotImplementedException();
        }

        public List<Folder> GetFolders(int folderId)
        {
            throw new NotImplementedException();
        }

        public void Rename(IFolder folder, string newName)
        {
            throw new NotImplementedException();
        }

        public void Delete(IFolder folder)
        {
            throw new NotImplementedException();
        }

        public Eav.Apps.Assets.Folder Get(int tenantId, string path)
        {
            throw new NotImplementedException();
        }
    }
}
