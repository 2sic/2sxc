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
            //_dynCode = dynCode;
            _block = block;
            Context = block.Context;
            _page = null;
            _module = null;
            return this;
        }

        private IBlock _block;

        #endregion

        public ICmsPlatform Platform { get; }

        public ICmsSite Site => Context.Site as ICmsSite;

        public ICmsPage Page => _page ?? (_page = (Context as IContextOfBlock)?.Page ?? new PageUnknown());
        private IPage _page;

        public ICmsCulture Culture => _culture ?? (_culture = new CmsCulture(this));
        private ICmsCulture _culture;

        public ICmsModule Module => _module ?? (_module = (Context as IContextOfBlock)?.Module ?? new ModuleUnknown());
        private IModule _module;

        public ICmsUser User => Context.User as ICmsUser;

        [PrivateApi]
        public ICmsView View => _view ?? (_view = new CmsView(_block));
        private ICmsView _view;
    }
}
