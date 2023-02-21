using ToSic.Eav.Helpers;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
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
        public string Path => _path.Get(() => FigureOutPath(_block?.App.Path));
        private readonly GetOnce<string> _path = new GetOnce<string>();

        /// <inheritdoc />
        public string PathShared => _pathShared.Get(() => FigureOutPath(_block?.App.PathShared));
        private readonly GetOnce<string> _pathShared = new GetOnce<string>();

        /// <inheritdoc />
        public string PhysicalPath => _physPath.Get(() => FigureOutPath(_block?.App.PhysicalPath));
        private readonly GetOnce<string> _physPath = new GetOnce<string>();

        /// <inheritdoc />
        public string PhysicalPathShared => _physPathShared.Get(() => FigureOutPath(_block?.App.PhysicalPathShared));
        private readonly GetOnce<string> _physPathShared = new GetOnce<string>();

        /// <summary>
        /// Figure out the path to the view based on a root path.
        /// </summary>
        /// <returns></returns>
        private string FigureOutPath(string root)
        {
            // Get addition, but must ensure it doesn't have a leading slash (otherwise Path.Combine treats it as a root)
            var addition = (UnwrappedContents.EditionPath ?? "").TrimPrefixSlash();
            var pathWithFile = System.IO.Path.Combine(root ?? "", addition).ForwardSlash();
            return pathWithFile.BeforeLast("/");
        }
    }
}
