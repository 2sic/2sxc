using ToSic.Eav.Context;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
    /// This lets the code be platform agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
    /// </summary>
    [PrivateApi("we only show the interface in the docs")]
    public class CmsContext: ICmsContext
    {

        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CmsContext(Platform platform, IContextOfSite context)
        {
            _context = context;
            Platform = platform;
        }
        private IContextOfSite _context;

        /// <summary>
        /// System to extend the known context by more information if we're running inside a block
        /// </summary>
        /// <param name="newContext"></param>
        /// <returns></returns>
        internal CmsContext Update(IContextOfSite newContext)
        {
            _context = newContext;
            _page = null;
            _module = null;
            return this;
        }

        #endregion

        public Platform Platform { get; }

        public ISiteLight Site => _context.Site as ISiteLight;

        public IPageLight Page => _page ?? (_page = (_context as IContextOfBlock)?.Page ?? new PageNull());
        private IPage _page;

        public IModuleLight Module =>
            _module ?? (_module = (_context as IContextOfBlock)?.Module ?? new ModuleNull());
        private IModule _module;

        public IUserLight User => _context.User as IUserLight;
    }
}
