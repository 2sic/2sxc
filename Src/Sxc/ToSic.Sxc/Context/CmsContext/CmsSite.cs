using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsSite: Wrapper<ISite>, ICmsSite
    {
        public CmsSite(ISite contents, AppState appState) : base(contents)
        {
            _appState = appState;
        }
        private readonly AppState _appState;

        public int Id => _contents?.Id ?? Eav.Constants.NullId;
        public string Url => _contents?.Url ?? string.Empty;
        public string UrlRoot => _contents.UrlRoot ?? string.Empty;

        public IMetadataOf Metadata
            => _metadata ?? (_metadata = _appState.GetMetadataOf((int)TargetTypes.CmsItem, CmsMetadata.SitePrefix + Id, Url));
        private IMetadataOf _metadata;

    }
}
