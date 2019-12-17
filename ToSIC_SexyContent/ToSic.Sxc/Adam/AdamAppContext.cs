using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using IApp = ToSic.Sxc.Apps.IApp;
using ICmsBlock = ToSic.Sxc.Blocks.ICmsBlock;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// The app-context of ADAM
    /// In charge of managing assets inside this app
    /// </summary>
    public class AdamAppContext: HasLog, IContext
    {
        /// <summary>
        /// the app is only used to get folder / guid etc.
        /// don't use it to access data! as the data should never have to be initialized for this to work
        /// always use the AppRuntime instead
        /// </summary>
        private readonly IApp _app;
        public readonly AppRuntime AppRuntime;
        public readonly ITenant Tenant;
        public readonly ICmsBlock CmsInstance;
        internal readonly IEnvironmentFileSystem EnvironmentFs;


        public AdamAppContext(ITenant tenant, IApp app, ICmsBlock cmsInstance, ILog parentLog) : base("Adm.ApCntx", parentLog, "starting")
        {
            Tenant = tenant;
            _app = app;
            AppRuntime = new AppRuntime(app, null);
            CmsInstance = cmsInstance;
            EnvironmentFs = Factory.Resolve<IEnvironmentFileSystem>();
        }

        /// <summary>
        /// Path to the app assets
        /// </summary>
        public string Path
            => _path ?? (_path =
                   Configuration.AppReplacementMap(_app)
                       .ReplaceInsensitive(Configuration.AdamAppRootFolder));
        private string _path;


        /// <summary>
        /// Root folder object of the app assets
        /// </summary>
        public Eav.Apps.Assets.Folder RootFolder => Folder(Path, true);

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


        internal Eav.Apps.Assets.Folder Folder(string path, bool autoCreate)
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


        internal Eav.Apps.Assets.Folder Folder(string path) => EnvironmentFs.Get(Tenant.Id, path, this);


        #endregion

        public Export Export => new Export(this);
    }
}