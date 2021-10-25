using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("Hide implementation")]
    public class CmsView: Wrapper<IView>, ICmsView, IWrapper<IView>
    {
        public CmsView(IBlock block): base(block?.View)
        {
            // UnwrappedContents = block?.View;
        }

        //[PrivateApi]
        //public IView UnwrappedContents { get; }

        /// <inheritdoc />
        public int Id => _contents?.Id ?? 0;

        /// <inheritdoc />
        public string Name => _contents?.Name ?? "";

        /// <inheritdoc />
        public string Identifier => _contents?.Identifier ?? "";

        /// <inheritdoc />
        public string Edition => _contents?.Edition;

    }
}
