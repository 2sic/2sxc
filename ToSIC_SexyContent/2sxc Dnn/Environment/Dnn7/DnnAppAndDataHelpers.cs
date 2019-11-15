using ToSic.Eav.Logging;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnAppAndDataHelpers : AppAndDataHelpersBase, IDynamicCode
    {
        public DnnAppAndDataHelpers(ICmsBlock cmsInstance, ILog parentLog = null): base(cmsInstance, new DnnTenant(null), parentLog)
        {
            // Init things than require module-info or similar, but not 2sxc
            var instance = cmsInstance?.Container;
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