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

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="blockBuilder">CMS Block which is used in this code.</param>
        /// <param name="compatibility">compatibility level - changes behaviour if level 9 or 10</param>
        /// <param name="parentLog">parent logger for logging what's happening</param>
        public DnnDynamicCode(IBlockBuilder blockBuilder, int compatibility = 10, ILog parentLog = null)
            : base(blockBuilder, new DnnTenant(null), compatibility, parentLog ?? blockBuilder?.Log)
        {
            // Init things than require module-info or similar, but not 2sxc
            var instance = blockBuilder?.Container;
            Dnn = new DnnContext(instance);
            Link = new DnnLinkHelper(Dnn);
        }

        #region IHasDnnContext

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn { get; }

        #endregion

        [PrivateApi]
        public override dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var instance = base.CreateInstance(virtualPath, dontRelyOnParameterOrder, name, relativePath, throwOnError);
            ForwardDynDnn(instance);
            return instance;
        }

        /// <summary>
        /// special: if it's a RazorComponent , then it doesn't have all the necessary properties yet...
        /// </summary>
        /// <param name="instance"></param>
        [PrivateApi]
        private void ForwardDynDnn(object instance)
        {
            if (instance is INeedsDynCodeContext needsDyn) needsDyn.DynCode = this;
        }

        #region Create From BlockBuilder

        [PrivateApi]
        internal static DnnDynamicCode Create(IBlockBuilder blockBuilder, ILog log)
            => new DnnDynamicCode(blockBuilder, 10, blockBuilder?.Log ?? log);

        #endregion

    }
}