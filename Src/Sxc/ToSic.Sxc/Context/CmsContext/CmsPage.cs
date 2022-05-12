using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("hide implementation")]
    public class CmsPage: Wrapper<IPage>, ICmsPage, IHasMetadata
    {
        public CmsPage(CmsContext parent, AppState appState) : base(parent?.CtxBlockOrNull?.Page as IPage ?? new PageUnknown(null))
        {
            _parent = parent;
            _appState = appState;
        }

        private readonly CmsContext _parent;
        private readonly AppState _appState;

        public int Id => _contents?.Id ?? 0;
        public IParameters Parameters => _contents?.Parameters;
        public string Url => _contents.Url ?? string.Empty;

        IDynamicMetadata ICmsPage.Metadata => _dynMeta.Get(() => new DynamicMetadata((this as IHasMetadata).Metadata, null, _parent.DEDeps));
        private readonly ValueGetOnce<IDynamicMetadata> _dynMeta = new ValueGetOnce<IDynamicMetadata>();


        IMetadataOf IHasMetadata.Metadata => _md.Get(() => _appState.GetMetadataOf(TargetTypes.Page, Id, Url));
        private readonly ValueGetOnce<IMetadataOf> _md = new ValueGetOnce<IMetadataOf>();
    }
}
