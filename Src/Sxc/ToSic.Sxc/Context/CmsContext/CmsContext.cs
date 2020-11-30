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
        /// <param name="newContext"></param>
        /// <returns></returns>
        internal CmsContext Update(IContextOfSite newContext)
        {
            Context = newContext;
            _page = null;
            _module = null;
            return this;
        }

        #endregion

        public Platform Platform { get; }

        public ISiteLight Site => Context.Site as ISiteLight;

        public IPageLight Page => _page ?? (_page = (Context as IContextOfBlock)?.Page ?? new PageNull());
        private IPage _page;

        public IModuleLight Module =>
            _module ?? (_module = (Context as IContextOfBlock)?.Module ?? new ModuleNull());
        private IModule _module;

        public IUserLight User => Context.User as IUserLight;
    }
}
