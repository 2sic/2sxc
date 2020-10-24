using System.Collections.Generic;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneRepositoryInfoFileSystem : RepositoryInfoOfFolder
    {
        #region Constructor and DI

        public OqtaneRepositoryInfoFileSystem(IServerPaths serverPaths) => _serverPaths = serverPaths;

        private readonly IServerPaths _serverPaths;

        #endregion

        public override List<string> RootPaths => new List<string>
        {
            _serverPaths.FullSystemPath("Modules/ToSic.Sxc/.data"),
            //@"c:\Projects\2sxc\octane\oqtane.framework\Oqtane.Server\wwwroot\Modules\ToSic.Sxc\.data"
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
