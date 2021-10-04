using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Code
{
    [PrivateApi]
    public class DnnDynamicCodeRoot : DynamicCodeRoot, Sxc.Code.IDynamicCode, IDnnDynamicCode, IHasDynamicCodeRoot
    {
        public DnnDynamicCodeRoot(Dependencies dependencies, IDnnContext dnnContext): base(dependencies, DnnConstants.LogName)
        {
            Dnn = dnnContext;
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="block">CMS Block which provides context and maybe some edit-allowed info.</param>
        /// <param name="parentLog">parent logger for logging what's happening</param>
        /// <param name="compatibility">compatibility level - changes behaviour if level 9 or 10</param>
        public override IDynamicCodeRoot Init(IBlock block, ILog parentLog, int compatibility = 10)
        {
            base.Init(block, parentLog, compatibility);
            // Init things than require module-info or similar, but not 2sxc
            ((DnnContextOld)Dnn).Init(block?.Context.Module);
            return this;
        }

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn { get; private set; }
    }
}