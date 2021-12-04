using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("hide implementation")]
    public class CmsPage: Wrapper<IPage>, ICmsPage
    {
        public CmsPage(IPage contents, AppState appState) : base(contents)
        {
            _appState = appState;
        }
        private readonly AppState _appState;

        public int Id => _contents?.Id ?? 0;
        public IParameters Parameters => _contents?.Parameters;
        public string Url => _contents.Url ?? string.Empty;

        public IMetadataOf Metadata
            => _metadata ?? (_metadata = _appState.GetMetadataOf((int)TargetTypes.CmsItem, CmsMetadata.PagePrefix + Id, Url));
        private IMetadataOf _metadata;

    }
}
