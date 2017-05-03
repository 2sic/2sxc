using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        internal void Add(string path)
        {
            try
            {
                folderManager.AddFolder(_portalId, path);
            }
            catch (SqlException)
            {
                // don't do anything - this happens when multiple processes try to add the folder at the same time
                // like when two fields in a dialog cause the web-api to create the folders in parallel calls
                // see also https://github.com/2sic/2sxc/issues/811
            }
            catch (NullReferenceException)
            {
                // also catch this, as it's an additional exception which also happens in the AddFolder when a folder already existed
            }
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