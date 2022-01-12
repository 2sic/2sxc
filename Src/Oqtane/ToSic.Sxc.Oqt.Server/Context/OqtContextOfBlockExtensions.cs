using Oqtane.Models;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public static class OqtContextOfBlockExtensions
    {
        public static IContextOfBlock Init(this IContextOfBlock context, int pageId, Module oqtModule, ILog parentLog)
        {
            context.Init(parentLog);
            ((OqtPage)context.Page).Init(pageId);
            ((OqtModule)context.Module).Init(oqtModule, parentLog);
            return context;
        }
    }
}
