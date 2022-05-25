using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Blocks;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsView: CmsContextPartBase<IView>, ICmsView
    {
        public CmsView(CmsContext parent, IBlock block): base(parent, block?.View) { }

        /// <inheritdoc />
        public int Id => _contents?.Id ?? 0;

        /// <inheritdoc />
        public string Name => _contents?.Name ?? "";

        /// <inheritdoc />
        public string Identifier => _contents?.Identifier ?? "";

        /// <inheritdoc />
        public string Edition => _contents?.Edition;

        protected override IMetadataOf GetMetadataOf()
            => ExtendWithRecommendations(_contents?.Metadata);

    }
}
