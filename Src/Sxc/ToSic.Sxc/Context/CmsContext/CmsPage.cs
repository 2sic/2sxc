using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("hide implementation")]
    public class CmsPage: CmsContextPartBase<IPage>, ICmsPage
    {
        public CmsPage(CmsContext parent, AppState appState, Lazy<IPage> fallbackPage)
            : base(parent, parent?.CtxBlockOrNull?.Page ?? fallbackPage.Value)
        {
            _appState = appState;
        }

        private readonly AppState _appState;

        public int Id => _contents?.Id ?? 0;
        public IParameters Parameters => _contents?.Parameters;
        public string Url => _contents.Url ?? string.Empty;

        protected override IMetadataOf GetMetadataOf()
            => ExtendWithRecommendations( _appState.GetMetadataOf(TargetTypes.Page, Id, Url));
    }
}
