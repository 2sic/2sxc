using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Razor.Code
{
    public class Razor3DynamicCode: Sxc.Code.DynamicCodeRoot
    {
        public Razor3DynamicCode(): base("Mvc.CodeRt") { }
        public Razor3DynamicCode Init(IBlock block, ILog parentLog) 
        {
            base.Init(block, 10, parentLog);
            return this;
        }
    }
}
