using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Code
{
    public class OqtaneDynamicCode: Sxc.Code.DynamicCodeRoot
    {
        public OqtaneDynamicCode(IServiceProvider serviceProvider, ICmsContext context): base(serviceProvider, context, OqtConstants.OqtLogPrefix)
        {
        }

        public OqtaneDynamicCode Init(IBlock block, ILog parentLog) 
        {
            base.Init(block, parentLog, 10);
            return this;
        }
    }
}
