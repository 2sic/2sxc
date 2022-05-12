using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
    /// This lets the code be platform agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
    /// </summary>
    [PrivateApi("we only show the interface in the docs")]
    public class CmsContext: HasLog, ICmsContext, INeedsDynamicCodeRoot
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CmsContext(IPlatform platform, IContextOfSite initialContext, IAppStates appStates) : base(Constants.SxcLogName + ".CmsCtx")
        {
            _initialContext = initialContext;
            _appStates = appStates;
            Platform = platform;
        }
        private readonly IContextOfSite _initialContext;

        internal IContextOfSite CtxSite => _ctxSite.Get(() => CtxBlockOrNull ?? _initialContext);
        private readonly ValueGetOnce<IContextOfSite> _ctxSite = new ValueGetOnce<IContextOfSite>();

        private readonly IAppStates _appStates;

        /// <summary>
        /// System to extend the known context by more information if we're running inside a block
        /// </summary>
        /// <returns></returns>
        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
        }

        //internal void AttachContext(IDynamicCodeRoot codeRoot)
        //{
        //    _codeRoot = codeRoot;
        //}

        internal IDynamicCodeRoot CodeRoot;
        internal DynamicEntityDependencies DEDeps => (CodeRoot as DynamicCodeRoot)?.DynamicEntityDependencies;

        private AppState SiteAppState => _siteAppState.Get(() => _appStates.GetPrimaryApp(CtxSite.Site.ZoneId, Log));
        private readonly ValueGetOnce<AppState> _siteAppState = new ValueGetOnce<AppState>();


        private IBlock RealBlock => _realBlock.Get(() => CodeRoot?.Block);
        private readonly ValueGetOnce<IBlock> _realBlock = new ValueGetOnce<IBlock>();

        internal IContextOfBlock CtxBlockOrNull => _ctxBlock.Get(() => CodeRoot?.Block?.Context);
        private readonly ValueGetOnce<IContextOfBlock> _ctxBlock = new ValueGetOnce<IContextOfBlock>();

        #endregion

        public ICmsPlatform Platform { get; }

        public ICmsSite Site => _site ?? (_site = new CmsSite(CtxSite.Site, SiteAppState));
        private ICmsSite _site;

        public ICmsPage Page => _page ?? (_page = new CmsPage(this, SiteAppState));
        private ICmsPage _page;

        public ICmsCulture Culture => _culture ?? (_culture = new CmsCulture(this));
        private ICmsCulture _culture;

        public ICmsModule Module => _cmsModule ?? (_cmsModule = new CmsModule(RealBlock.Context?.Module ?? new ModuleUnknown(null), RealBlock));
        private ICmsModule _cmsModule;

        public ICmsUser User => _user ?? (_user = new CmsUser(CtxSite.User, SiteAppState));
        private ICmsUser _user;

        public ICmsView View => _view ?? (_view = new CmsView(RealBlock));
        private ICmsView _view;

        public ICmsBlock Block => _cmsBlock ?? (_cmsBlock = new CmsBlock(RealBlock));
        private ICmsBlock _cmsBlock;
    }
}
