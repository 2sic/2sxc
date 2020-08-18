using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.Sxc.Dnn.Code
{
    public class DnnDynamicCode : Sxc.Code.DynamicCodeRoot, IDnnDynamicCode
    {
        [PrivateApi]
        public new IBlockBuilder BlockBuilder => base.BlockBuilder;

        public DnnDynamicCode(): base("Dnn.DynCdRt") { }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="blockBuilder">CMS Block which is used in this code.</param>
        /// <param name="parentLog">parent logger for logging what's happening</param>
        /// <param name="compatibility">compatibility level - changes behaviour if level 9 or 10</param>
        public DnnDynamicCode Init(IBlockBuilder blockBuilder, ILog parentLog = null, int compatibility = 10)
            // : base(blockBuilder, new DnnTenant(null), compatibility, parentLog ?? blockBuilder?.Log)
        {
            base.Init(blockBuilder, blockBuilder?.Context.Tenant ?? new DnnTenant(null), compatibility, parentLog ?? blockBuilder?.Log);
            // Init things than require module-info or similar, but not 2sxc
            var instance = blockBuilder?.Context.Container;
            Dnn = new DnnContextOld(instance);
            Link = new DnnLinkHelper(Dnn);
            return this;
        }

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn { get; private set; }

    }
}