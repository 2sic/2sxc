using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;


namespace ToSic.Sxc.Hybrid.Code
{
    public class HybridDynamicCode: Sxc.Code.DynamicCodeRoot, IHybridDynamicCode
    {
        public HybridDynamicCode(Dependencies dependencies): base(dependencies, "HybDynCode") { }

        public HybridDynamicCode Init(IBlock block, ILog parentLog)
        {
            base.Init(block, parentLog, 10);
            return this;
        }
    }
}
