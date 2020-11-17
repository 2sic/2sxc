using System.Collections.Generic;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtRepositoryInfoFileSystem : RepositoryInfoOfFolder
    {
        #region Constructor and DI

        public OqtRepositoryInfoFileSystem(IServerPaths serverPaths) => _serverPaths = serverPaths;

        private readonly IServerPaths _serverPaths;

        #endregion

        public override List<string> RootPaths => new List<string>
        {
            _serverPaths.FullSystemPath("Modules/ToSic.Sxc/.data"),
        };
    }
}
