using ToSic.Eav.Helpers;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
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

        /// <inheritdoc />
        public string Path => _path.Get(() => CombinePathsAndForward(_block?.App.Path, UnwrappedContents.Edition));
        private readonly GetOnce<string> _path = new GetOnce<string>();

        /// <inheritdoc />
        public string PathShared => _pathShared.Get(() => CombinePathsAndForward(_block?.App.PathShared, UnwrappedContents.Edition));
        private readonly GetOnce<string> _pathShared = new GetOnce<string>();

        /// <inheritdoc />
        public string PhysicalPath => _physPath.Get(() => CombinePathsAndForward(_block?.App.PhysicalPath, UnwrappedContents.Edition));
        private readonly GetOnce<string> _physPath = new GetOnce<string>();

        /// <inheritdoc />
        public string PhysicalPathShared => _physPathShared.Get(() => CombinePathsAndForward(_block?.App.PhysicalPathShared, UnwrappedContents.Edition));
        private readonly GetOnce<string> _physPathShared = new GetOnce<string>();

        private string CombinePathsAndForward(string path, string addition)
            => System.IO.Path.Combine(path ?? "", addition ?? "").ForwardSlash();
    }
}
