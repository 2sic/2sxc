using static ToSic.Eav.Constants;

namespace ToSic.Sxc.Code.Context
{
    public class ContextBundle
    {
        #region Constructor

        internal ContextBundle(DynamicCodeRoot root)
        {
            _root = root;
        }
        private readonly DynamicCodeRoot _root;

        #endregion

        public ContextPlatform Platform;

        public ContextSite Site =>
            _site ?? (_site = new ContextSite {Id = _root?.Block?.Context?.Tenant.Id ?? NullId});
        private ContextSite _site;

        public ContextPage Page => _page ?? (_page = new ContextPage {Id = _root?.Block?.Context?.Page.Id ?? NullId});
        private ContextPage _page;

        public ContextModule Module =>
            _module ?? (_module = new ContextModule {Id = _root?.Block?.Context?.Container?.Id ?? NullId});
        private ContextModule _module;
    }
}
