using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps
{
    public class AppFolder: HasLog<AppFolder>
    {
        public AppFolder(IContextResolver ctxResolver) : base("AppFolder") 
            => _ctxResolver = ctxResolver;
        private readonly IContextResolver _ctxResolver;

        public string GetAppFolder()
        {
            var ctx = _ctxResolver.BlockOrNull();
            return ctx.AppState.Folder;
        }
    }
}
