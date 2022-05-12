using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsSite: CmsContextPartBase<ISite>, ICmsSite
    {
        public CmsSite(CmsContext parent, AppState appState) : base(parent, parent.CtxSite.Site)
        {
            _appState = appState;
        }

        private readonly AppState _appState;

        public int Id => _contents?.Id ?? Eav.Constants.NullId;
        public string Url => _contents?.Url ?? string.Empty;
        public string UrlRoot => _contents.UrlRoot ?? string.Empty;

        protected override IMetadataOf GetMetadataOf() 
            => _appState.GetMetadataOf(TargetTypes.Site, Id, Url);
    }
}
