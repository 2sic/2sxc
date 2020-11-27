using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
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

        internal CmsContext Update(IContextOfSite newContext)
        {
            _context = newContext;
            _page = null;
            _module = null;
            return this;
        }

        private ICmsContext _cmsContextImplementation;

        #endregion

        public Platform Platform { get; }

        //public SiteContext Site =>
        //    _site ?? (_site = new SiteContext {Id = _root?.Block?.Context?.Site.Id ?? NullId});
        //private SiteContext _site;

        //public PageContext Page => _page ?? (_page = new PageContext {Id = _root?.Block?.Context?.Page.Id ?? NullId});
        //private PageContext _page;

        //public ModuleContext Module =>
        //    _module ?? (_module = new ModuleContext {Id = _root?.Block?.Context?.Container?.Id ?? NullId});
        //private ModuleContext _module;

        public ISiteLight Site => _context.Site;

        public IPageLight Page => _page ?? (_page = (_context as IContextOfBlock)?.Page ?? new PageNull());
        private IPage _page;

        public IModuleLight Module =>
            _module ?? (_module = (_context as IContextOfBlock)?.Module ?? new ModuleNull());
        private IModule _module;

        public IUserLight User => _context.User;
    }
}
