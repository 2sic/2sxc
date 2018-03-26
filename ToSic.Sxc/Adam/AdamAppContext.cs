using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The app-context of ADAM
    /// In charge of managing assets inside this app
    /// </summary>
    public class AdamAppContext
    {
        public readonly App App;
        public readonly ITenant Tenant;
        public readonly SxcInstance SxcInstance;
        internal readonly IEnvironmentFileSystem EnvironmentFs;


        public AdamAppContext(ITenant tenant, App app, SxcInstance sxcInstance)
        {
            Tenant = tenant;
            App = app;
            SxcInstance = sxcInstance;
            EnvironmentFs = Factory.Resolve<IEnvironmentFileSystem>();
        }

        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path
            => _path ?? (_path =
                   Configuration.AppReplacementMap(App)
                       .ReplaceInsensitive(Configuration.AdamAppRootFolder));
        private string _path;


        /// <summary>
        /// Root folder object of the app assets
        /// </summary>
        public Folder RootFolder => Folder(Path, true);

        #region basic, generic folder commands -- all internal

        /// <summary>
        /// Verify that a path exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal bool Exists(string path) => EnvironmentFs.FolderExists(Tenant.Id, path);

        /// <summary>
        /// Create a path (folder)
        /// </summary>
        /// <param name="path"></param>
        internal void Add(string path) => EnvironmentFs.AddFolder(Tenant.Id, path);


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

            var fldr = Folder(path);

            return fldr;
        }


        internal Folder Folder(string path) => EnvironmentFs.Get(Tenant.Id, path, this);


        #endregion

        public Export Export => new Export(this);


        //public string UrlPath(AdamFile currentFile)
        //    => Tenant.ContentPath + currentFile.Folder + currentFile.FileName;

        //public string UrlPath(AdamFolder currentFolder)
        //    => Tenant.ContentPath + currentFolder.FolderPath;

    }
}