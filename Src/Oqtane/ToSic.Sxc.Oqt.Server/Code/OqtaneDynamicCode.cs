using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Code
{
    // TODO: remove, because we will use dynamic code hybrid implementation
    public class OqtaneDynamicCode: Sxc.Code.DynamicCodeRoot /*, IOqtaneDynamicCode*/
    {
        public OqtaneDynamicCode(Dependencies dependencies): base(dependencies, OqtConstants.OqtLogPrefix) { }

        public OqtaneDynamicCode Init(IBlock block, ILog parentLog)
        {
            base.Init(block, parentLog, 10);
            return this;
        }
    }
}
