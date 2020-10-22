using System.Collections.Generic;
using ToSic.Eav.Repositories;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    public class OqtaneRepositoryInfoFileSystem : RepositoryInfoOfFolder
    {
        #region Constructor and DI

        public OqtaneRepositoryInfoFileSystem(IHttp http) : base() => _http = http;

        private readonly IHttp _http;

        #endregion

        public override List<string> RootPaths => new List<string>
        {
            _http.MapPath("wwwroot/Modules/ToSic.Sxc/.data"),
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
