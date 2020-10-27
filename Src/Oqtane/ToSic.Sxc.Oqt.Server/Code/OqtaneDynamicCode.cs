using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Oqt.Server.Code
{
    public class OqtaneDynamicCode: Sxc.Code.DynamicCodeRoot
    {
        public OqtaneDynamicCode(): base("Mvc.CodeRt") { }
        public OqtaneDynamicCode Init(IBlock block, ILog parentLog) 
        {
            base.Init(block, parentLog, 10);
            return this;
        }
    }
}
