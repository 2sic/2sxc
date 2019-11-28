using ToSic.Eav.Logging;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Dnn
{
    public class DynamicCodeHelper : Web.DynamicCodeHelper, IDynamicCode
    {
        public ICmsBlock CmsBlock;

        public DynamicCodeHelper(ICmsBlock cmsBlock, ILog parentLog = null): base(cmsBlock, new DnnTenant(null), parentLog)
        {
            CmsBlock = cmsBlock;
            // Init things than require module-info or similar, but not 2sxc
            var instance = cmsBlock?.Container;
            Dnn = new DnnHelper(instance);
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