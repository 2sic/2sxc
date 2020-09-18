using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Mvc.Code
{
    public class MvcDynamicCode: Sxc.Code.DynamicCodeRoot
    {
        public MvcDynamicCode(): base("Mvc.CodeRt") { }
        public MvcDynamicCode Init(IBlock block, ILog parentLog) 
        {
            base.Init(block, 10, parentLog);
            return this;
        }
    }
}
