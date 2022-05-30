using System;
using ToSic.Eav.Context;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    public class ContextResolver: HasLog<IContextResolver>, IContextResolver
    {
        #region Constructor / DI
        protected readonly LazyInitLog<AppIdResolver> AppIdResolver;

        private IServiceProvider ServiceProvider { get; }

        public ContextResolver(IServiceProvider serviceProvider, LazyInitLog<AppIdResolver> appIdResolverLazy) : base("Sxc.CtxRes")
        {
            AppIdResolver = appIdResolverLazy.SetLog(Log);
            ServiceProvider = serviceProvider;
        }

        #endregion

        public IContextOfSite Site() => _site ?? (_site = ServiceProvider.Build<IContextOfSite>());
        private IContextOfSite _site;

        public IContextOfApp App(int appId)
        {
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
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

        private IContextOfBlock BlockContext => _blockContext ?? (_blockContext = RealBlockOrNull()?.Context);// _getBlockContext?.Invoke());
        private IContextOfBlock _blockContext;

        //public void AttachBlockContext(Func<IContextOfBlock> getBlockContext) => _getBlockContext = getBlockContext;
        //private Func<IContextOfBlock> _getBlockContext;

        public void AttachRealBlock(Func<IBlock> getBlock) => _getBlock = getBlock;
        private Func<IBlock> _getBlock;

        public IBlock RealBlockOrNull() => _getBlock?.Invoke();

        public IBlock RealBlockRequired() => _getBlock?.Invoke() ?? throw new Exception("Block required but missing. It was not attached");


        public IContextOfApp App(string nameOrPath) => App(AppIdResolver.Ready.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, true));

        public IContextOfApp AppOrBlock(string nameOrPath) => AppOrNull(nameOrPath) ?? BlockRequired();

        public IContextOfApp AppOrNull(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
            var id = AppIdResolver.Ready.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, false);
            return id <= Eav.Constants.AppIdEmpty ? null : App(id);
        }

        public IContextOfApp AppNameRouteBlock(string nameOrPath)
        {
            var ctx = AppOrNull(nameOrPath);
            if (ctx != null) return ctx;

            var identity = AppIdResolver.Ready.GetAppIdFromRoute();
            if (identity != null)
            {
                ctx = ServiceProvider.Build<IContextOfApp>();
                ctx.Init(Log);
                ctx.ResetApp(identity);
                return ctx;
            }

            ctx = BlockOrNull();
            return ctx ?? throw new Exception($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
        }


    }
}
