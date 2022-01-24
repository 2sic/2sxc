using ToSic.Eav.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Apps
{
    public class OqtAppFolder: HasLog<OqtAppFolder>
    {
        public OqtAppFolder(IContextResolver ctxResolver) : base($"{OqtConstants.OqtLogPrefix}.AppFolder") 
            => _ctxResolver = ctxResolver;
        private readonly IContextResolver _ctxResolver;

        public string GetAppFolder()
        {
            var ctx = _ctxResolver.BlockOrNull();
            return ctx.AppState.Folder;
        }
    }
}
