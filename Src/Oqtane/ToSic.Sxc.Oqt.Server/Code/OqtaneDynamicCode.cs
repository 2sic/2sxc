using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Code
{
    public class OqtaneDynamicCode: Sxc.Code.DynamicCodeRoot
    {
        public OqtaneDynamicCode(Dependencies dependencies): base(dependencies, OqtConstants.OqtLogPrefix) { }

        public OqtaneDynamicCode Init(IBlock block, ILog parentLog) 
        {
            base.Init(block, parentLog, 10);
            return this;
        }
    }
}
