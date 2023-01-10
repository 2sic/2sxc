using Oqtane.Models;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public static class OqtContextOfBlockExtensions
    {
        public static IContextOfBlock Init(this IContextOfBlock context, int pageId, Module oqtModule)
        {
            ((OqtPage)context.Page).Init(pageId);
            ((OqtModule)context.Module).Init(oqtModule);
            return context;
        }
    }
}
