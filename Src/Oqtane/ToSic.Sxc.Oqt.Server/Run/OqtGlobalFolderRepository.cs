//using System.Collections.Generic;
//using ToSic.Eav.Repositories;
//using ToSic.Eav.Run;

//namespace ToSic.Sxc.Oqt.Server.Run
//{
//    public class OqtGlobalFolderRepository : FolderBasedRepository
//    {
//        #region Constructor and DI

//        public OqtGlobalFolderRepository(IServerPaths serverPaths) => _serverPaths = serverPaths;

//        private readonly IServerPaths _serverPaths;

//        #endregion

//        public override List<string> RootPaths => new List<string>
//        {
//            // todo: fix, as FullSystemPath should now come from Configuration
//            // _serverPaths.FullSystemPath("Modules/ToSic.Sxc/.data"),
//        };
//    }
//}
