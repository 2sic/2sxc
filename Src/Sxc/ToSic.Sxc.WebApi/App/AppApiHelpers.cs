//using ToSic.Eav;
//using ToSic.Eav.Apps;
//using ToSic.Eav.Logging;
//using ToSic.Sxc.Blocks;
//using ToSic.Sxc.LookUp;
//using IApp = ToSic.Sxc.Apps.IApp;

//namespace ToSic.Sxc.WebApi.App
//{
//    public static class AppApiHelpers
//    {
//        /// <summary>
//        /// used for API calls to get the current app
//        /// </summary>
//        /// <returns></returns>
//        public static IApp GetApp(int appId, IBlockBuilder blockBuilder, ILog log)
//        {
//            var appIdentity = new AppIdentity(SystemRuntime.ZoneIdOfApp(appId), appId);
//            var app = Factory.Resolve<Apps.App>().Init(appIdentity, ConfigurationProvider.Build(blockBuilder, true),
//                false, log);
//            return app;
//        }

//    }
//}
