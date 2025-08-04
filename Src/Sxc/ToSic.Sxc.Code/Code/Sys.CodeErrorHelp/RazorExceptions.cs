namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;
public class RazorExceptions
{

    #region Exceptions to throw in various not-supported cases

    public static dynamic ExAsDynamicForList()
        => throw new("AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");

    internal static dynamic ExCreateSourceString()
        => throw new($"CreateSource(string, ...) {HelpDbRazor.IsNotSupportedIn12Plus}. Please use CreateSource<DataSourceTypeName>(...) instead.");

    private static dynamic ExNotYetSupported(string original, string recommended)
        => throw new($"{original} {HelpDbRazor.IsNotSupportedIn12Plus}. Use {recommended}.");

    public static object ExAsDynamicKvp() => ExNotYetSupported("AsDynamic(KeyValuePair<int, IEntity>", "AsDynamic(IEnumerable<IEntity>...)");

    public static object ExPresentation() => ExNotYetSupported("Presentation", "Content.Presentation");
    public static object ExListPresentation() => ExNotYetSupported("ListPresentation", "Header.Presentation");
    public static object ExListContent() => ExNotYetSupported("ListContent", "Header");
    public static IEnumerable<object> ExList() => ExNotYetSupported("List", "Data[\"Default\"].List");

    public static object ExAsDynamicInterfacesIEntity()
        =>
            ExNotYetSupported($"AsDynamic(Eav.Interfaces.IEntity)", "Please cast your data to ToSic.Eav.Data.IEntity.");

    public static object AsDynamicKvpInterfacesIEntity()
        =>
            ExNotYetSupported("AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity>)",
                "Please cast your data to ToSic.Eav.Data.IEntity.");

    public static IEnumerable<object> AsDynamicIEnumInterfacesIEntity()
        =>
            ExNotYetSupported("AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities)",
                "Please cast your data to ToSic.Eav.Data.IEntity.");

    #endregion
}
