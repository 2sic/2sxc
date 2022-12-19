using System;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Custom.Hybrid
{
    [PrivateApi]
    public static class Obsolete10
    {
        private const string NotSupportedIn10 = "is not supported in Razor14, Razor12 or RazorComponent (v10)";

        internal static dynamic AsDynamicForList()
            => throw new Exception("AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");

        internal static dynamic CreateSourceString()
            => throw new Exception($"CreateSource(string, ...) {NotSupportedIn10}. Please use CreateSource<DataSourceTypeName>(...) instead.");

        private static dynamic NotSupported(string original, string recommended)
            => throw new Exception($"{original} {NotSupportedIn10}. Use {recommended}.");

        public static dynamic AsDynamicKvp() => NotSupported("AsDynamic(KeyValuePair<int, IEntity>", "AsDynamic(IEnumerable<IEntity>...)");
        public static dynamic Presentation() => NotSupported("Presentation", "Content.Presentation");
        public static dynamic ListPresentation() => NotSupported("ListPresentation", "Header.Presentation");
        public static dynamic ListContent() => NotSupported("ListContent", "Header");
        public static dynamic List() => NotSupported("List", "Data[\"DefaultAuthenticationEventArgs\"].List");

        public static dynamic AsDynamicInterfacesIEntity()
            => NotSupported($"AsDynamic(Eav.Interfaces.IEntity)", "Please cast your data to ToSic.Eav.Data.IEntity.");

        public static dynamic AsDynamicKvpInterfacesIEntity()
            => NotSupported("AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity>)", "Please cast your data to ToSic.Eav.Data.IEntity.");

        public static dynamic AsDynamicIEnumInterfacesIEntity()
            => NotSupported("AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities)", "Please cast your data to ToSic.Eav.Data.IEntity.");
    }
}
