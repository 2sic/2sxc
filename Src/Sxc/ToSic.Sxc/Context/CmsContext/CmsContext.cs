using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
    /// This lets the code be platform agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
    /// </summary>
    [PrivateApi("we only show the interface in the docs")]
    public class CmsContext: ServiceForDynamicCode, ICmsContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CmsContext(
            IPlatform platform, 
            IContextOfSite initialContext, 
            Lazy<IPage> pageLazy,
            IAppStates appStates,
            Lazy<ICmsSite> cmsSiteLazy
        ) : base(Constants.SxcLogName + ".CmsCtx")
        {
            _initialContext = initialContext;
            _pageLazy = pageLazy;
            _appStates = appStates;
            _cmsSiteLazy = cmsSiteLazy;
            Platform = platform;
        }
        private readonly IContextOfSite _initialContext;
        private readonly Lazy<IPage> _pageLazy;

        internal IContextOfSite CtxSite => _ctxSite.Get(() => CtxBlockOrNull ?? _initialContext);
        private readonly GetOnce<IContextOfSite> _ctxSite = new GetOnce<IContextOfSite>();

        private readonly IAppStates _appStates;
        private readonly Lazy<ICmsSite> _cmsSiteLazy;

        ///// <summary>
        ///// System to extend the known context by more information if we're running inside a block
        ///// </summary>
        ///// <returns></returns>
        //public void ConnectToRoot(IDynamicCodeRoot codeRoot) => CodeRoot = codeRoot;
        //protected IDynamicCodeRoot CodeRoot;

        internal DynamicEntityDependencies DEDeps => (_DynCodeRoot as DynamicCodeRoot)?.DynamicEntityDependencies;

        private AppState SiteAppState => _siteAppState.Get(() => _appStates.GetPrimaryApp(CtxSite.Site.ZoneId, Log));
        private readonly GetOnce<AppState> _siteAppState = new GetOnce<AppState>();


        private IBlock RealBlockOrNull => _realBlock.Get(() => _DynCodeRoot?.Block);
        private readonly GetOnce<IBlock> _realBlock = new GetOnce<IBlock>();

        internal IContextOfBlock CtxBlockOrNull => _ctxBlock.Get(() => _DynCodeRoot?.Block?.Context);
        private readonly GetOnce<IContextOfBlock> _ctxBlock = new GetOnce<IContextOfBlock>();

        #endregion

        public ICmsPlatform Platform { get; }

        public ICmsSite Site => _site.Get(() => _cmsSiteLazy.Value.Init(this, SiteAppState));
        private readonly GetOnce<ICmsSite> _site = new GetOnce<ICmsSite>();

        public ICmsPage Page => _page ?? (_page = new CmsPage(this, SiteAppState, _pageLazy));
        private ICmsPage _page;

        public ICmsCulture Culture => _culture ?? (_culture = new CmsCulture(this));
        private ICmsCulture _culture;

        public ICmsModule Module => _cmsModule ?? (_cmsModule = new CmsModule(this, RealBlockOrNull.Context?.Module ?? new ModuleUnknown(null), RealBlockOrNull));
        private ICmsModule _cmsModule;

        public ICmsUser User => _user ?? (_user = new CmsUser(this, SiteAppState));
        private ICmsUser _user;

        public ICmsView View => _view ?? (_view = new CmsView(this, RealBlockOrNull));
        private ICmsView _view;

        public ICmsBlock Block => _cmsBlock ?? (_cmsBlock = new CmsBlock(RealBlockOrNull));
        private ICmsBlock _cmsBlock;
    }
}
