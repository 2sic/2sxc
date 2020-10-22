using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.OqtaneModule.Server.Code
{
    public class OqtaneDynamicCode: Sxc.Code.DynamicCodeRoot
    {
        public OqtaneDynamicCode(): base("Mvc.CodeRt") { }
        public OqtaneDynamicCode Init(IBlock block, ILog parentLog) 
        {
            base.Init(block, 10, parentLog);
            return this;
        }
    }
}
