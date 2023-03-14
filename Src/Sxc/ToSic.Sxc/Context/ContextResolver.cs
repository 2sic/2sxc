using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Context
{
    public partial class ContextResolver: Eav.Context.ContextResolver, IContextResolver
    {
        #region Constructor / DI
        
        protected readonly LazySvc<AppIdResolver> AppIdResolver;
        //private readonly Generator<IContextOfApp> _contextOfApp;

        public ContextResolver(
            LazySvc<AppIdResolver> appIdResolverLazy,
            Generator<IContextOfSite> contextOfSite,
            Generator<IContextOfApp> contextOfApp) : base(contextOfSite, contextOfApp, "Sxc.CtxRes")
        {
            ConnectServices(
                //_contextOfApp = contextOfApp,
                AppIdResolver = appIdResolverLazy
            );
        }

        #endregion


        /// <summary>
        /// Get the best possible context which can give us insights about the user permissions.
        ///
        /// TODO: WIP - requires that if an app is to be used, it was accessed before - not yet perfect...
        /// </summary>
        /// <returns></returns>
        public IContextOfUserPermissions UserPermissions() => _ctxUserPerm.Get(() => BlockContext ?? LatestAppContext ?? Site());
        private readonly GetOnce<IContextOfUserPermissions> _ctxUserPerm = new GetOnce<IContextOfUserPermissions>();
    }
}
