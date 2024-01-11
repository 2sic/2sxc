using ToSic.Eav.Code.Help;
using static ToSic.Sxc.Code.Internal.CodeErrorHelp.CodeHelpBuilder;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HelpForRazor12
{
    #region Methods to build Help DB

    internal const string IsNotSupportedIn12Plus = "is not supported in Razor12+";

    internal static CodeHelp HelpNotExists12(string property, params string[] replacement)
        => new GenNotExist(property, replacement)
        {
            MsgNotSupportedIn = IsNotSupportedIn12Plus
        }.Generate();

    #endregion

    /// <summary>
    /// List re-used in v12 and v14
    /// </summary>
    internal static List<CodeHelp> Issues12To14 => _help12And14 ??= BuildListFromDiverseSources(
        // Access .List
        BuildVariations(
            HelpNotExists12("List", "AsDynamic(Data)"),
            h => new(h, detect: "does not contain a definition for 'List'"),
            h => new(h,
                detect:
                @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments")
        ),

        // Access .ListContent
        HelpNotExists12("ListContent", "Header"),
        HelpNotExists12("ListPresentation", "Header.Presentation"),

        // .Presentation
        HelpNotExists12("Presentation", "Content.Presentation"),

        // Use Dnn
        DnnObjectNotInHybrid,

        // .CreateSource(string) - Obsolete
        new CodeHelp(name: "CreateSource-String-Obsolete",
            detect:
            @"error CS0411: The type arguments for method .*\.CreateSource.*cannot be inferred from the usage",
            detectRegex: true,
            linkCode:
            "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.Services.IDataService.html#ToSic_Sxc_Services_IDataService_GetSource_",
            uiMessage: $@"
You are probably calling CreateSource(stringNameOfSource, ...) which {IsNotSupportedIn12Plus}. 
",
            detailsHtml: $@"
You are probably calling <code>CreateSource(stringNameOfSource, ...)</code> which {IsNotSupportedIn12Plus}. Use: 
<ol>
    <li>Kit.Data.GetSource&lt;TypeName&gt;(...)</li>
    <li>Kit.Data.GetSource(appDataSourceName, ...)</li>
</ol>
")

        // Not handled - can't because the AsDynamic accepts IEntity which works in Razor14
        // dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
        // dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
        // IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
        // dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => Obsolete10.AsDynamicKvp();

        // Skipped, as can't be detected - they are all IEnumerable...
        //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataStream stream) => Obsolete10.AsDynamicForList();
        //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IDataSource source) => Obsolete10.AsDynamicForList();
        //[PrivateApi] public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => Obsolete10.AsDynamicForList();
    );
    private static List<CodeHelp> _help12And14;

    /// <summary>
    /// All issues for v12 - ATM identical with the shared list.
    /// </summary>
    internal static List<CodeHelp> Compile12 => Issues12To14;


    #region Help which is used in various places

    internal static CodeHelp DnnObjectNotInHybrid = new(name: "Object-Dnn-Not-In-Hybrid",
        detect: @"error CS0118: 'Dnn' is a 'namespace' but is used like a 'variable'",
        uiMessage: $@"
You are probably trying to use the 'Dnn' object which is not supported in 'Custom.Hybrid.Razor' templates. 
",
        detailsHtml: $@"
You are probably trying to use the <code>Dnn</code> object which is not supported in <code>Custom.Hybrid.Razor</code> templates. Use: 
<ol>
    <li>Other APIs such as <code>CmsContext</code> to get page/module etc. information</li>
    <li>If really necessary (not recommended) use the standard Dnn APIs to get the necessary objects.</li>
</ol>
");
    #endregion


    #region Exceptions to throw in various not-supported cases

    internal static dynamic ExAsDynamicForList()
        => throw new("AsDynamic for lists isn't supported in RazorComponent. Please use AsList instead.");

    internal static dynamic ExCreateSourceString()
        => throw new($"CreateSource(string, ...) {IsNotSupportedIn12Plus}. Please use CreateSource<DataSourceTypeName>(...) instead.");

    private static dynamic ExNotYetSupported(string original, string recommended)
        => throw new($"{original} {IsNotSupportedIn12Plus}. Use {recommended}.");

    public static object ExAsDynamicKvp() =>
        ExNotYetSupported("AsDynamic(KeyValuePair<int, IEntity>", "AsDynamic(IEnumerable<IEntity>...)");

    public static object ExPresentation() => ExNotYetSupported("Presentation", "Content.Presentation");
    public static object ExListPresentation() => ExNotYetSupported("ListPresentation", "Header.Presentation");
    public static object ExListContent() => ExNotYetSupported("ListContent", "Header");
    public static IEnumerable<object> ExList() => ExNotYetSupported("List", "Data[\"Default\"].List");

    public static object ExAsDynamicInterfacesIEntity()
        => ExNotYetSupported($"AsDynamic(Eav.Interfaces.IEntity)", "Please cast your data to ToSic.Eav.Data.IEntity.");

    public static object AsDynamicKvpInterfacesIEntity()
        => ExNotYetSupported("AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity>)",
            "Please cast your data to ToSic.Eav.Data.IEntity.");

    public static IEnumerable<object> AsDynamicIEnumInterfacesIEntity()
        => ExNotYetSupported("AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities)",
            "Please cast your data to ToSic.Eav.Data.IEntity.");

    #endregion

}