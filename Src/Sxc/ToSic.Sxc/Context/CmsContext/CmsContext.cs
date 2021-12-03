using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

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
        public CmsContext(IPlatform platform, IContextOfSite context)
        {
            Context = context;
            Platform = platform;
        }

        /// <summary>
        /// This is the real context - in case other things need to re-use it somewhere.
        /// But we're not showing it on the public ICmsContext, as that's a very internal feature
        /// which could change.
        /// Note that this can contain an IContextOfSite, or an IContextOfBlock
        /// </summary>
        public IContextOfSite Context;

        /// <summary>
        /// System to extend the known context by more information if we're running inside a block
        /// </summary>
        /// <returns></returns>
        internal CmsContext Update(IBlock block)
        {
            _block = block;
            Context = block.Context;
            return this;
        }

        private IBlock _block;

        #endregion

        public ICmsPlatform Platform { get; }

        public ICmsSite Site => _site ?? (_site = new CmsSite(Context.Site, _block.Context?.AppState));
        private ICmsSite _site;

        public ICmsPage Page => _page ?? (_page = new CmsPage((Context as IContextOfBlock)?.Page ?? new PageUnknown(null), _block.Context?.AppState));
        private ICmsPage _page;

        public ICmsCulture Culture => _culture ?? (_culture = new CmsCulture(this));
        private ICmsCulture _culture;

        public ICmsModule Module => _cmsModule ?? (_cmsModule = new CmsModule((Context as IContextOfBlock)?.Module ?? new ModuleUnknown(null), _block));
        private ICmsModule _cmsModule;

        public ICmsUser User => _user ?? (_user = new CmsUser(Context.User, _block.Context?.AppState)); // Context.User as ICmsUser;
        private ICmsUser _user;

        public ICmsView View => _view ?? (_view = new CmsView(_block));
        private ICmsView _view;

        public ICmsBlock Block => _cmsBlock ?? (_cmsBlock = new CmsBlock(_block));
        private ICmsBlock _cmsBlock;
    }
}
