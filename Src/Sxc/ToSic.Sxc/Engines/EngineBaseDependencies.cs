using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Engines
{
    public class EngineBaseDependencies: DependenciesBase<EngineBaseDependencies>
    {

        public EngineBaseDependencies(IServerPaths serverPaths,
            IBlockResourceExtractor blockResourceExtractor,
            Lazy<AppPermissionCheck> appPermCheckLazy,
            Polymorphism.Polymorphism polymorphism,
            Lazy<IAppStates> appStatesLazy
        ) => AddToLogQueue(
            Polymorphism = polymorphism,
            AppStatesLazy = appStatesLazy,
            ServerPaths = serverPaths,
            BlockResourceExtractor = blockResourceExtractor,
            AppPermCheckLazy = appPermCheckLazy
        );

        internal readonly IServerPaths ServerPaths;
        internal readonly IBlockResourceExtractor BlockResourceExtractor;
        internal readonly Lazy<AppPermissionCheck> AppPermCheckLazy;
        internal Polymorphism.Polymorphism Polymorphism { get; }
        internal Lazy<IAppStates> AppStatesLazy { get; }
    }
}
