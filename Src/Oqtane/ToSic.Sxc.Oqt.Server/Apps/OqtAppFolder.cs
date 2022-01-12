using ToSic.Eav.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtAppFolder: HasLog<OqtAppFolder>
    {
        // TODO: @STV - pls test, I changed OqtState to use ctxResolver
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
