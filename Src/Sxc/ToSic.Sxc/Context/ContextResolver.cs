using System;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helper;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    public class ContextResolver: ServiceBase, IContextResolver
    {
        #region Constructor / DI
        
        protected readonly LazySvc<AppIdResolver> AppIdResolver;
        private readonly Generator<IContextOfSite> _contextOfSite;
        private readonly Generator<IContextOfApp> _contextOfApp;

        public ContextResolver(
            LazySvc<AppIdResolver> appIdResolverLazy,
            Generator<IContextOfSite> contextOfSite,
            Generator<IContextOfApp> contextOfApp) : base("Sxc.CtxRes")
        {
            ConnectServices(
                _contextOfSite = contextOfSite,
                _contextOfApp = contextOfApp,
                AppIdResolver = appIdResolverLazy
                );
        }

        #endregion

        public IContextOfSite Site() => _site.Get(() => _contextOfSite.New());
        private readonly GetOnce<IContextOfSite> _site = new GetOnce<IContextOfSite>();

        public IContextOfApp App(int appId)
        {
            var appContext = _contextOfApp.New();
            //appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        public IContextOfBlock BlockRequired() => BlockContext ?? throw new Exception("Block context required but not known. It was not attached.");

        public IContextOfBlock BlockOrNull() => BlockContext;

        public IContextOfApp BlockOrApp(int appId)
        {
            // get the current block context
            var ctx = BlockContext;

            // if there is a block context, make sure it's of the requested app (or no app was specified)
            // then return that
            // note: an edge case is that a block context exists, but no app was selected - then AppState is null
            if(ctx != null && (appId == 0 || appId == ctx.AppState?.AppId)) return ctx;

            // if block was found but we're working on another app (like through app-admin)
            // then ignore block permissions / context and only return app
            return App(appId);
        }

        private IContextOfBlock BlockContext => _blockContext.Get(() => RealBlockOrNull()?.Context);// _getBlockContext?.Invoke());
        private readonly GetOnce<IContextOfBlock> _blockContext = new GetOnce<IContextOfBlock>();

        //public void AttachBlockContext(Func<IContextOfBlock> getBlockContext) => _getBlockContext = getBlockContext;
        //private Func<IContextOfBlock> _getBlockContext;

        public void AttachRealBlock(Func<IBlock> getBlock) => _getBlock = getBlock;
        private Func<IBlock> _getBlock;

        public IBlock RealBlockOrNull() => _getBlock?.Invoke();

        public IBlock RealBlockRequired() => _getBlock?.Invoke() ?? throw new Exception("Block required but missing. It was not attached");


        public IContextOfApp App(string nameOrPath) => App(AppIdResolver.Value.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, true));

        public IContextOfApp AppOrBlock(string nameOrPath) => AppOrNull(nameOrPath) ?? BlockRequired();

        public IContextOfApp AppOrNull(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
            var id = AppIdResolver.Value.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, false);
            return id <= Eav.Constants.AppIdEmpty ? null : App(id);
        }

        public IContextOfApp AppNameRouteBlock(string nameOrPath)
        {
            var ctx = AppOrNull(nameOrPath);
            if (ctx != null) return ctx;

            var identity = AppIdResolver.Value.GetAppIdFromRoute();
            if (identity != null)
            {
                ctx = _contextOfApp.New();
                //ctx.Init(Log);
                ctx.ResetApp(identity);
                return ctx;
            }

            ctx = BlockOrNull();
            return ctx ?? throw new Exception($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
        }


    }
}
