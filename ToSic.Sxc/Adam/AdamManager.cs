using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Assets;
using ToSic.SexyContent;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam
{
    public class AdamManager
    {
        private readonly App _app;
        private readonly int _tenantId;
        public const string AdamAppRootFolder = "adam/[AppFolder]/";
        private readonly AdamBrowseContext _browseContext;

        public AdamManager(int tenantId, App app, AdamBrowseContext browseContext = null)
        {
            _tenantId = tenantId;
            _app = app;
            _browseContext = browseContext;
            EnvironmentFs = Factory.Resolve<IEnvironmentFileSystem>();
        }

        public string RootPath => AdamAppRootFolder.Replace("[AppFolder]", _app.Folder);

        public Folder Root => Folder(RootPath, true);

        #region basic, generic folder commands -- all internal

        internal readonly IEnvironmentFileSystem EnvironmentFs;
        internal bool Exists(string path) => EnvironmentFs.FolderExists(_tenantId, path);

        internal void Add(string path) => EnvironmentFs.AddFolder(_tenantId, path);

        internal Folder Get(string path) => EnvironmentFs.Get(_tenantId, path, _browseContext);

        internal Folder Folder(string path, bool autoCreate)
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

            var fldr = Get(path);

            return fldr;
        }

        #endregion

        public Export Export => new Export(this);



    }
}