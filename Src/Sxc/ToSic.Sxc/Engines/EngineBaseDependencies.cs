using System;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class EngineBaseDependencies
    {
        public EngineBaseDependencies(IServerPaths serverPaths, 
            ILinkPaths linkPaths, 
            TemplateHelpers templateHelpers, 
            IClientDependencyOptimizer clientDependencyOptimizer,
            Lazy<AppPermissionCheck> appPermCheckLazy)
        {
            ServerPaths = serverPaths;
            LinkPaths = linkPaths;
            TemplateHelpers = templateHelpers;
            ClientDependencyOptimizer = clientDependencyOptimizer;
            AppPermCheckLazy = appPermCheckLazy;
        }

        internal readonly IServerPaths ServerPaths;
        internal readonly ILinkPaths LinkPaths;
        internal readonly TemplateHelpers TemplateHelpers;
        internal readonly IClientDependencyOptimizer ClientDependencyOptimizer;
        internal readonly Lazy<AppPermissionCheck> AppPermCheckLazy;
    }
}
