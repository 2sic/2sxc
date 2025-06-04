using ToSic.Sys.Code.Help;
using static ToSic.Sxc.Code.Internal.CodeErrorHelp.CodeHelpBuilder;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
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
    internal static List<CodeHelp> Issues12To14 => field ??= BuildListFromDiverseSources(
        // Access .List
        BuildVariations(
            HelpNotExists12("List", "AsDynamic(Data)"),
            h => h with
            {
                Detect = "does not contain a definition for 'List'",
            },
            h => h with 
            {
                Detect = @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments",
            }
        ),

        // Access .ListContent
        HelpNotExists12("ListContent", "Header"),
        HelpNotExists12("ListPresentation", "Header.Presentation"),

        // .Presentation
        HelpNotExists12("Presentation", "Content.Presentation"),

        // Use Dnn
        DnnObjectNotInHybrid,

    #region New v20 warnings

        // New v20 - detect use of using ToSic.Eav.Interfaces
        new CodeHelp
        {
            Name = "Using-ToSic.Eav.Interfaces",
            Detect = @"error CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'ToSic.Eav' (are you missing an assembly reference?)",
            UiMessage = "You are probably using the old namespace ToSic.Eav.Interfaces, which is not supported since v20. Replace \"ToSic.Eav.Interfaces\" with \"ToSic.Eav.Data\" in your code.",
        },
        new CodeHelp
        {
            Name = "Using-ToSic.SexyContent.Interfaces",
            Detect = @"error CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'ToSic.SexyContent' (are you missing an assembly reference?)",
            UiMessage = "You are probably using the old namespace ToSic.SexyContent.Interfaces, which is not supported since v20. Please remove/replace according to upgrade guide TODO!.",
        },

        // New v20 - detect usage of `IEntity` without the namespace 
        new CodeHelp
        {
            Name = "IEntity-Without-Namespace",
            Detect = @"error CS0246: The type or namespace name 'IEntity' could not be found (are you missing a using directive or an assembly reference?)",
            UiMessage = "You are probably using IEntity in your code, but missing the @using ToSic.Eav.Data",
        },

        // New v20 - detect usage of `IEntityLight` - but not clear why it could think it exists, since it shouldn't - but in my tests it tried...
        new CodeHelp
        {
            Name = "Detect Convert to IEntityLight",
            // Full error is something like "Unable to cast object of type 'ToSic.Eav.Data.Entity' to type 'ToSic.Eav.Data.IEntityLight'."
            Detect = @"to type 'ToSic.Eav.Data.IEntityLight'",
            UiMessage = "Your code seems to use an old interface IEntityLight. Best just use 'ToSic.Eav.Data.IEntity' or see if the conversion is even necessary.",
        },

        // New v20 - detect usage of `IEntityLight` which should not exist in any DLLs any more
        new CodeHelp
        {
            Name = "Detect missing IEntityLight",
            // Full error is something like: "error CS0246: The type or namespace name 'IEntityLight' could not be found (are you missing a using directive or an assembly reference?)"
            Detect = @"error CS0246: The type or namespace name 'IEntityLight' could not be found",
            UiMessage = "Your code seems to use an old interface IEntityLight. Best just use 'ToSic.Eav.Data.IEntity' or see if the conversion is even necessary",
        },

        // New v20 - detect usage of `GetBestValue(name, languages, bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old GetBestValue - without the parameter name",
            // Full error is something like: "error CS1501: No overload for method 'GetBestValue' takes 3 arguments at System.Web.Compilation.AssemblyBuilder.Compile()"
            Detect = @"error CS1501: No overload for method 'GetBestValue' takes 3 arguments",
            UiMessage = "Your code seems to use an old 'GetBestValue() overload to get with/without converting to links. This has been inactive for a long time and is removed in v20. Please see guide TODO!",
        },

        // New v20 - detect usage of `GetBestValue(name, languages, resolveHyperlinks: bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old GetBestValue - with the parameter name",
            // Full error is something like: "error CS1739: The best overload for 'GetBestValue' does not have a parameter named 'resolveHyperlinks' at System.Web.Compilation.AssemblyBuilder.Compile()"
            Detect = @"error CS1739: The best overload for 'GetBestValue' does not have a parameter named 'resolveHyperlinks'",
            UiMessage = "Your code seems to use an old 'GetBestValue() overload to get with/without converting to links. This has been inactive for a long time and is removed in v20. Please see guide TODO!",
        },

        // New v20 - detect usage of `PrimaryValue(name, languages, resolveHyperlinks: bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old PrimaryValue / PrimaryValue<T>",
            // Full error is something like: "error CS1061: 'IEntity' does not contain a definition for 'PrimaryValue' and no accessible extension method 'PrimaryValue' accepting a first argument of type 'IEntity' could be found (are you missing a using directive or an assembly reference?) at System.Web.Compilation.AssemblyBuilder.Compile()"
            Detect = @"error CS1061: 'IEntity' does not contain a definition for 'PrimaryValue'",
            UiMessage = "Your code seems to use an old 'PrimaryValue() method to get values. This has is removed in v20. Please use 'Get(...)' instead.",
        },

    #endregion


        // .CreateSource(string) - Obsolete
        new CodeHelp
        {
            Name = "CreateSource-String-Obsolete",
            Detect = @"error CS0411: The type arguments for method .*\.CreateSource.*cannot be inferred from the usage",
            DetectRegex = true,
            LinkCode = "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.Services.IDataService.html#ToSic_Sxc_Services_IDataService_GetSource_",
            UiMessage = $@"
You are probably calling CreateSource(stringNameOfSource, ...) which {IsNotSupportedIn12Plus}. 
",
            DetailsHtml = $@"
You are probably calling <code>CreateSource(stringNameOfSource, ...)</code> which {IsNotSupportedIn12Plus}. Use: 
<ol>
    <li>Kit.Data.GetSource&lt;TypeName&gt;(...)</li>
    <li>Kit.Data.GetSource(appDataSourceName, ...)</li>
</ol>
"
        }

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

    /// <summary>
    /// All issues for v12 - ATM identical with the shared list.
    /// </summary>
    internal static List<CodeHelp> Compile12 => Issues12To14;


    #region Help which is used in various places

    internal static CodeHelp DnnObjectNotInHybrid = new()
    {
        Name = "Object-Dnn-Not-In-Hybrid",
        Detect = @"error CS0118: 'Dnn' is a 'namespace' but is used like a 'variable'",
        UiMessage = $@"
You are probably trying to use the 'Dnn' object which is not supported in 'Custom.Hybrid.Razor' templates. 
",
        DetailsHtml = $@"
You are probably trying to use the <code>Dnn</code> object which is not supported in <code>Custom.Hybrid.Razor</code> templates. Use: 
<ol>
    <li>Other APIs such as <code>CmsContext</code> to get page/module etc. information</li>
    <li>If really necessary (not recommended) use the standard Dnn APIs to get the necessary objects.</li>
</ol>
"
    };
    #endregion


    #region Exceptions to throw in various not-supported cases

    public static dynamic ExAsDynamicForList()
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