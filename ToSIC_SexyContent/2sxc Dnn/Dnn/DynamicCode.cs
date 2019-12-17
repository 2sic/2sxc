using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Dnn
{
    public class DynamicCode : Sxc.Web.DynamicCode, IDynamicCode
    {
        public ICmsBlock CmsBlock;

        public DynamicCode(ICmsBlock cmsBlock, ILog parentLog = null): base(cmsBlock, new DnnTenant(null), parentLog)
        {
            CmsBlock = cmsBlock;
            // Init things than require module-info or similar, but not 2sxc
            var instance = cmsBlock?.Container;
            Dnn = new DnnContext(instance);
            Link = new DnnLinkHelper(Dnn);
        }

        #region IHasDnnContext

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn { get; }

        #endregion


    }
}