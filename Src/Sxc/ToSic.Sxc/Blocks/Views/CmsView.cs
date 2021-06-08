using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("Hide implementation")]
    public class CmsView: ICmsView, IWrapper<IView>
    {
        public CmsView(IBlock block)
        {
            UnwrappedContents = block?.View;
        }

        [PrivateApi]
        public IView UnwrappedContents { get; }

        /// <inheritdoc />
        public int Id => UnwrappedContents?.Id ?? 0;

        /// <inheritdoc />
        public string Name => UnwrappedContents?.Name ?? "";

        /// <inheritdoc />
        public string Identifier => UnwrappedContents?.Identifier ?? "";

        /// <inheritdoc />
        public string Edition => UnwrappedContents?.Edition;

    }
}
