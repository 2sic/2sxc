﻿using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Engines
{
    public class EngineBaseServices: MyServicesBase
    {

        public EngineBaseServices(IServerPaths serverPaths,
            IBlockResourceExtractor blockResourceExtractor,
            LazySvc<AppPermissionCheck> appPermCheckLazy,
            Polymorphism.Polymorphism polymorphism,
            LazySvc<IAppStates> appStatesLazy
        )
        {
            ConnectServices(
                Polymorphism = polymorphism,
                AppStatesLazy = appStatesLazy,
                ServerPaths = serverPaths,
                BlockResourceExtractor = blockResourceExtractor,
                AppPermCheckLazy = appPermCheckLazy
            );
        }

        internal readonly IServerPaths ServerPaths;
        internal readonly IBlockResourceExtractor BlockResourceExtractor;
        internal readonly LazySvc<AppPermissionCheck> AppPermCheckLazy;
        internal Polymorphism.Polymorphism Polymorphism { get; }
        internal LazySvc<IAppStates> AppStatesLazy { get; }
    }
}