using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using static ToSic.Eav.Constants;

namespace ToSic.Sxc.Run.Context
{
    /// <summary>
    /// Runtime context information, used in dynamic code. Help the code to detect what environment it's in, what page etc.
    /// This lets the code be platform agnostic, so that it works across implementations (Dnn, Oqtane, NopCommerce)
    /// </summary>
    [WorkInProgressApi("Still WIP")]
    public class RunContext
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="platform"></param>
        public RunContext(PlatformContext platform) => Platform = platform;

        internal RunContext Init(DynamicCodeRoot root)
        {
            _root = root;
            return this;
        }
        private DynamicCodeRoot _root;

        #endregion

        public PlatformContext Platform { get; }

        public SiteContext Site =>
            _site ?? (_site = new SiteContext {Id = _root?.Block?.Context?.Site.Id ?? NullId});
        private SiteContext _site;

        public PageContext Page => _page ?? (_page = new PageContext {Id = _root?.Block?.Context?.Page.Id ?? NullId});
        private PageContext _page;

        public ModuleContext Module =>
            _module ?? (_module = new ModuleContext {Id = _root?.Block?.Context?.Container?.Id ?? NullId});
        private ModuleContext _module;
    }
}
