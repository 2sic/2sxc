using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.Sxc.Dnn.Code
{
    public class DnnDynamicCode : Sxc.Code.DynamicCodeRoot, IDnnDynamicCode, IHasDynCodeContext
    {
        public DnnDynamicCode(): base("Dnn.DynCdRt") { }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="block">CMS Block which provides context and maybe some edit-allowed info.</param>
        /// <param name="parentLog">parent logger for logging what's happening</param>
        /// <param name="compatibility">compatibility level - changes behaviour if level 9 or 10</param>
        public DnnDynamicCode Init(IBlock block, ILog parentLog, int compatibility = 10)
        {
            Init(block, compatibility, parentLog);
            // Init things than require module-info or similar, but not 2sxc
            Dnn = new DnnContextOld(block?.Context.Container);
            Link = new DnnLinkHelper(Dnn);
            return this;
        }

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn { get; private set; }

        public DnnDynamicCode DynCode => this;
    }
}