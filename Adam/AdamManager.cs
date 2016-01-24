using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class AdamManager
    {
        private App _app;
        private int _portalId;
        public const string AdamAppRootFolder = "adam/[AppFolder]/";


        public AdamManager(int portalId, App app)
        {
            _portalId = portalId;
            _app = app;
        }

        public string RootPath => AdamAppRootFolder.Replace("[AppFolder]", _app.Folder);

        public IFolderInfo Root => Folder(RootPath, true);

        #region basic, generic foldor commands -- all internal
        private IFolderManager folderManager = FolderManager.Instance;
        internal bool Exists(string path)
        {
            return folderManager.FolderExists(_portalId, path);
        }
        internal IFolderInfo Add(string path)
        {
            return folderManager.AddFolder(_portalId, path);
        }

        internal IFolderInfo Get(string path)
        {
            return folderManager.GetFolder(_portalId, path);
        }

        internal IFolderInfo Folder(string path, bool autoCreate)
        {
            IFolderInfo fldr;

            //var path = GeneratePath(subFolder);

            // create all folders to ensure they exist. Must do one-by-one because dnn must have it in the catalog
            var pathParts = path.Split('/');
            var pathToCheck = ""; // pathParts[0];
            foreach (string part in pathParts.Where(p => !String.IsNullOrEmpty(p)))
            {
                pathToCheck += part + "/";
                if (Exists(pathToCheck)) continue;
                if (autoCreate)
                    Add(pathToCheck);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "subfolder " + pathToCheck + "not found" });
            }

            fldr = Get(path);

            return fldr;
        }

        #endregion

        public Export Export => new Export(this);



    }
}