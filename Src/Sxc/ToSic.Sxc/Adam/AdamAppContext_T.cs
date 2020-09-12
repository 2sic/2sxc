using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Adam
{
    public class AdamAppContext<TFolderId, TFileId>: AdamAppContext
    {
        public AdamAppContext(ITenant tenant, IApp app, IBlock block, int compatibility, ILog parentLog) 
            : base(tenant, app, block, compatibility, parentLog)
        {
            AdamFs = Factory.Resolve<IAdamFileSystem<TFolderId, TFileId>>().Init(this);

        }
        public IAdamFileSystem<TFolderId, TFileId> AdamFs { get; protected set; }

        /// <summary>
        /// Root folder object of the app assets
        /// </summary>
        public Folder<TFolderId, TFileId> RootFolder => Folder(Path, true);

        /// <summary>
        /// Verify that a path exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal bool Exists(string path) => AdamFs.FolderExists(path);

        /// <summary>
        /// Create a path (folder)
        /// </summary>
        /// <param name="path"></param>
        internal void Add(string path) => AdamFs.AddFolder(path);

        internal Folder<TFolderId, TFileId> Folder(string path) => AdamFs.Get(path);

        internal Folder<TFolderId, TFileId> Folder(string path, bool autoCreate)
        {
            // create all folders to ensure they exist. Must do one-by-one because the environment must have it in the catalog
            var pathParts = path.Split('/');
            var pathToCheck = "";
            foreach (var part in pathParts.Where(p => !string.IsNullOrEmpty(p)))
            {
                pathToCheck += part + "/";
                if (Exists(pathToCheck)) continue;
                if (autoCreate)
                    Add(pathToCheck);
                else
                    throw new Exception("subfolder " + pathToCheck + "not found");
            }

            return Folder(path);
        }

    }
}
