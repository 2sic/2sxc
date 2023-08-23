using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Identity;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;
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
            LazySvc<IPage> pageLazy,
            IAppStates appStates,
            LazySvc<ICmsSite> cmsSiteLazy
        ) : base(Constants.SxcLogName + ".CmsCtx")
        {
            ConnectServices(
                _initialContext = initialContext,
                _pageLazy = pageLazy,
                _appStates = appStates,
                _cmsSiteLazy = cmsSiteLazy,
                Platform = platform
            );
        }
        private readonly IContextOfSite _initialContext;
        private readonly LazySvc<IPage> _pageLazy;

        internal IContextOfSite CtxSite => _ctxSite.Get(() => CtxBlockOrNull ?? _initialContext);
        private readonly GetOnce<IContextOfSite> _ctxSite = new GetOnce<IContextOfSite>();

        private readonly IAppStates _appStates;
        private readonly LazySvc<ICmsSite> _cmsSiteLazy;

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

        #region Unique Key

        /// <summary>
        /// A unique, random key for the current module.
        /// It's recommended for giving DOM elements a unique id for scripts to then access them.
        /// 
        /// It's generated for every content-block, and more reliable than `Module.Id`
        /// since that sometimes results in duplicate keys, if the many blocks are used inside each other.
        ///
        /// It's generated using a GUID and converted/shortened. 
        /// In the current version it's 8 characters long, so it has 10^14 combinations, making collisions extremely unlikely.
        /// (currently 8 characters)
        /// </summary>
        [PrivateApi]
        public string UniqueKey => _uniqueKey ?? (_uniqueKey = GenerateUniqueId());
        private string _uniqueKey;

        [PrivateApi]
        public string GenerateUniqueId() => Guid.NewGuid().GuidCompress().Substring(0, 8);

        #endregion
    }
}
