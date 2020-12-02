using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    public class ContextResolver: HasLog<IContextResolver>, IContextResolver
    {
        private IServiceProvider ServiceProvider { get; }

        public ContextResolver(IServiceProvider serviceProvider) : base("Sxc.CtxRes")
        {
            ServiceProvider = serviceProvider;
        }

        public IContextOfSite Site() => ServiceProvider.Build<IContextOfSite>();

        public IContextOfApp App(int appId)
        {
            var appContext = ServiceProvider.Build<IContextOfApp>();
            appContext.Init(Log);
            appContext.ResetApp(appId);
            return appContext;
        }

        public IContextOfBlock BlockRequired() => BlockContext ?? throw new Exception("Block context required but not known. It was not attached.");

        public IContextOfBlock BlockOrNull() => BlockContext;

        public IContextOfApp BlockOrApp(int appId) => BlockContext ?? App(appId);

        private IContextOfBlock BlockContext => _blockContext ?? (_blockContext = _getBlockContext?.Invoke());
        private IContextOfBlock _blockContext;

        public void AttachBlockContext(Func<IContextOfBlock> getBlockContext) => _getBlockContext = getBlockContext;
        private Func<IContextOfBlock> _getBlockContext;

        public void AttachRealBlock(Func<IBlock> getBlock) => _getBlock = getBlock;
        private Func<IBlock> _getBlock;

        public IBlock RealBlockRequired() => _getBlock?.Invoke() ??
                                             throw new Exception("Block required but not known. It was not attached");

        public IContextOfApp App(string appPathOrName) => App(GetAppIdFromPath(appPathOrName, true));

        public IContextOfApp AppOrNull(string appPathOrName)
        {
            var id = GetAppIdFromPath(appPathOrName, false);
            return id <= Eav.Constants.AppIdEmpty ? null : App(id);
        }

        private int GetAppIdFromPath(string appPath, bool required)
        {
            var wrapLog = Log.Call<int>(appPath);
            var zid = Site().Site.ZoneId;
            // get app from AppName
            var aid = new ZoneRuntime().Init(zid, Log).FindAppId(appPath, true);
            if(aid <= Eav.Constants.AppIdEmpty && required)
                throw new Exception($"App required but can't find App based on the name '{appPath}'");

            return wrapLog($"found app:{aid}", aid);
        }

    }
}
