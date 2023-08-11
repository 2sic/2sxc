using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests
{
    internal static class TypedItemTestAccessors
    {
        public static IMetadata TestMetadata(this ITypedItem item) => item.Metadata;
    }
}
