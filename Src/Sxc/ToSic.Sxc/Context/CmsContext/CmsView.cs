using ToSic.Eav.Helpers;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsView: CmsContextPartBase<IView>, ICmsView
    {
        public CmsView(CmsContext parent, IBlock block) : base(parent, block?.View)
        {
            _block = block;
        }

        private IBlock _block;

        /// <inheritdoc />
        public int Id => UnwrappedContents?.Id ?? 0;

        /// <inheritdoc />
        public string Name => UnwrappedContents?.Name ?? "";

        /// <inheritdoc />
        public string Identifier => UnwrappedContents?.Identifier ?? "";

        /// <inheritdoc />
        public string Edition => UnwrappedContents?.Edition;

        protected override IMetadataOf GetMetadataOf()
            => ExtendWithRecommendations(UnwrappedContents?.Metadata);

        // 2022-06-22 this was a idea, but not enabled yet, as not really clear if this is useful
        //public string Path => $"{_block?.App.Path}{_contents.EditionPath}".ForwardSlash();
    }
}
