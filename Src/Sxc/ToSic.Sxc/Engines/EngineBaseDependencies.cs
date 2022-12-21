using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Engines
{
    public class EngineBaseDependencies: ServiceDependencies
    {

        public EngineBaseDependencies(IServerPaths serverPaths,
            IBlockResourceExtractor blockResourceExtractor,
            ILazySvc<AppPermissionCheck> appPermCheckLazy,
            Polymorphism.Polymorphism polymorphism,
            ILazySvc<IAppStates> appStatesLazy
        ) => AddToLogQueue(
            Polymorphism = polymorphism,
            AppStatesLazy = appStatesLazy,
            ServerPaths = serverPaths,
            BlockResourceExtractor = blockResourceExtractor,
            AppPermCheckLazy = appPermCheckLazy
        );

        internal readonly IServerPaths ServerPaths;
        internal readonly IBlockResourceExtractor BlockResourceExtractor;
        internal readonly ILazySvc<AppPermissionCheck> AppPermCheckLazy;
        internal Polymorphism.Polymorphism Polymorphism { get; }
        internal ILazySvc<IAppStates> AppStatesLazy { get; }
    }
}
