using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class EngineBaseDependencies
    {

        public EngineBaseDependencies(IServerPaths serverPaths, 
            ILinkPaths linkPaths, 
            TemplateHelpers templateHelpers, 
            IBlockResourceExtractor blockResourceExtractor,
            Lazy<AppPermissionCheck> appPermCheckLazy,
            Polymorphism.Polymorphism polymorphism,
            Lazy<IAppStates> appStatesLazy)
        {
            Polymorphism = polymorphism;
            AppStatesLazy = appStatesLazy;
            ServerPaths = serverPaths;
            LinkPaths = linkPaths;
            TemplateHelpers = templateHelpers;
            BlockResourceExtractor = blockResourceExtractor;
            AppPermCheckLazy = appPermCheckLazy;
        }

        internal readonly IServerPaths ServerPaths;
        internal readonly ILinkPaths LinkPaths;
        internal readonly TemplateHelpers TemplateHelpers;
        internal readonly IBlockResourceExtractor BlockResourceExtractor;
        internal readonly Lazy<AppPermissionCheck> AppPermCheckLazy;
        internal Polymorphism.Polymorphism Polymorphism { get; }
        internal Lazy<IAppStates> AppStatesLazy { get; }
    }
}
