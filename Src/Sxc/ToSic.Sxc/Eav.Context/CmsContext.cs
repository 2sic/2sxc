using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using static ToSic.Eav.Constants;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    /// <summary>
    /// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
    /// This lets the code be platform agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
    /// </summary>
    [WorkInProgressApi("Still WIP")]
    public class CmsContext: ICmsContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="platform"></param>
        public CmsContext(Platform platform) => Platform = platform;

        internal CmsContext Init(DynamicCodeRoot root)
        {
            _root = root;
            return this;
        }
        private DynamicCodeRoot _root;
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

        public ISiteLight Site => _cmsContextImplementation.Site;

        public IPageLight Page => _cmsContextImplementation.Page;

        public IModuleLight Module => _cmsContextImplementation.Module;

        public IUserLight User => _cmsContextImplementation.User;
    }
}
