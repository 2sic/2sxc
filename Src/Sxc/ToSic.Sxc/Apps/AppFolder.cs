using System;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps
{
    public class AppFolder: ServiceBase
    {
        public AppFolder(IContextResolver ctxResolver) : base("AppFolder") 
            => _ctxResolver = ctxResolver;
        private readonly IContextResolver _ctxResolver;

        public AppFolder Init(Func<IBlock> getBlock)
        {
            _ctxResolver.AttachRealBlock(getBlock);
            return this;
        }

        public string GetAppFolder()
        {
            var ctx = _ctxResolver.AppNameRouteBlock("");
            return ctx.AppState.Folder;
        }
    }
}
