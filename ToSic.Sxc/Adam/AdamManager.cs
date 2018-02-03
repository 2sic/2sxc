using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.Apps.Assets;

namespace ToSic.SexyContent.Adam
{
    public class AdamManager
    {
        private readonly App _app;
        private readonly int _tennantId;
        public const string AdamAppRootFolder = "adam/[AppFolder]/";
        private readonly AdamBrowseContext _browseContext;

        public AdamManager(int tennantId, App app, AdamBrowseContext browseContext = null)
        {
            _tennantId = tennantId;
            _app = app;
            _browseContext = browseContext;
        }

        public string RootPath => AdamAppRootFolder.Replace("[AppFolder]", _app.Folder);

        public Folder Root => Folder(RootPath, true);

        #region basic, generic foldor commands -- all internal
        //private readonly IFolderManager _folderManager = FolderManager.Instance;
        private readonly DnnFileSystem dnnfs = new DnnFileSystem();
        internal bool Exists(string path) => dnnfs.FolderExists(_tennantId, path);

        internal void Add(string path) => dnnfs.AddFolder(_tennantId, path);

        internal Folder Get(string path) => dnnfs.Get(_tennantId, path, _browseContext);

        internal Folder Folder(string path, bool autoCreate)
        {
            // create all folders to ensure they exist. Must do one-by-one because dnn must have it in the catalog
            var pathParts = path.Split('/');
            var pathToCheck = "";
            foreach (string part in pathParts.Where(p => !string.IsNullOrEmpty(p)))
            {
                pathToCheck += part + "/";
                if (Exists(pathToCheck)) continue;
                if (autoCreate)
                    Add(pathToCheck);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "subfolder " + pathToCheck + "not found" });
            }

            var fldr = Get(path);

            return fldr;
        }

        #endregion

        public Export Export => new Export(this);



    }
}