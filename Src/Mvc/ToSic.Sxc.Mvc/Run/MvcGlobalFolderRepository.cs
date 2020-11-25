using System.Collections.Generic;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcGlobalFolderRepository: FolderBasedRepository
    {
        #region Constructor and DI

        public MvcGlobalFolderRepository(IServerPaths serverPaths) => _serverPaths = serverPaths;

        private readonly IServerPaths _serverPaths;

        #endregion

        public override List<string> RootPaths => new List<string>
        {
            _serverPaths.FullSystemPath("wwwroot/System/Sxc/.data"),
            //"C:\\Projects\\poc-2sxc-mvc-website\\Website\\wwwroot\\System\\Sxc\\.data"
            //BuildPath(Constants.FolderData),
            //BuildPath(Constants.FolderDataBeta),
            //BuildPath(Constants.FolderDataCustom)
        };

        //private string BuildPath(string pathEnd) =>
        //    HostingEnvironment.MapPath(System.IO.Path.Combine(Eav.ImportExport.Settings.ModuleDirectory,
        //        pathEnd));

    }
}
