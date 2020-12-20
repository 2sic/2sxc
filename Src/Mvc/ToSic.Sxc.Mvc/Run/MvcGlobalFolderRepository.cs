//using System.Collections.Generic;
//using ToSic.Eav.Repositories;
//using ToSic.Eav.Run;

//namespace ToSic.Sxc.Mvc.Run
//{
//    public class MvcGlobalFolderRepository: FolderBasedRepository
//    {
//        #region Constructor and DI

//        public MvcGlobalFolderRepository(IServerPaths serverPaths) => _serverPaths = serverPaths;

//        private readonly IServerPaths _serverPaths;

//        #endregion

//        public override List<string> RootPaths => new List<string>
//        {
//            // todo: fix, as FullSystemPath should now come from Configuration
//            //_serverPaths.FullSystemPath("wwwroot/System/Sxc/.data"),
//        };
//    }
//}
