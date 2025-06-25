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

    [field: AllowNull, MaybeNull]
    internal static List<CodeHelp> IssuesRazor12AlsoForRuntime => field ??= BuildListFromDiverseSources(

        #region New v20 warnings

        // New v20 - detect usage of `PrimaryValue(name, languages, resolveHyperlinks: bool)` which does not exist anymore
        new CodeHelp
        {
            Name = "Detect use of old PrimaryValue / PrimaryValue<T>",
            // Full error is something like: "error CS1061: 'IEntity' does not contain a definition for 'PrimaryValue' and no accessible extension method 'PrimaryValue' accepting a first argument of type 'IEntity' could be found (are you missing a using directive or an assembly reference?) at System.Web.Compilation.AssemblyBuilder.Compile()"
            // But it can also come from a non IEntity object, like from a 'ToSic.Eav.Data.EntityDecorators.Sys.EntityWithDecorator<ToSic.Sxc.Data.Internal.Decorators.EntityInBlockDecorator>'
            Detect = @"does not contain a definition for 'PrimaryValue'",
            UiMessage = "Your code seems to use an old 'PrimaryValue(...) method to get values. This has is removed in v20. Please use 'Get(...)' instead.",
        },

        new CodeHelp
        {
            Name = "Detect use of old Value / Value<T>",
            // Full error is something like: Error: Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: 'ToSic.Eav.Data.EntityDecorators.Sys.EntityWithDecorator<ToSic.Sxc.Data.Internal.Decorators.EntityInBlockDecorator>' does not contain a definition for 'Value' at CallSite.Target(Closure , CallSite , Object , String ) at
            // note that "at" has a few spaces in front of it
            Detect = @"does not contain a definition for 'Value'",
            UiMessage = "Your code seems to use an old 'Value(...) method to get values. This has is removed in v20. Please use 'Get(...)' instead.",
        }

        #endregion

    );

    // New v20 - detect usage of `GetBestValue(...)` which does not exist anymore
    public static CodeHelp GetBestValueGone = new CodeHelp
    {
        Name = "Detect use of old GetBestValue - any which stopped existing",
        // Full error is something like: "ToSic.Eav.Data.EntityDecorators.Sys.EntityWithDecorator<ToSic.Sxc.Data.Sys.Decorators.CmsEditDecorator>' does not contain a definition for 'GetBestValue' at CallSite"
        Detect = @"does not contain a definition for 'GetBestValue'",
        UiMessage =
            "Your code seems to use an old 'GetBestValue() overload to get with/without converting to links. Please use Get(...) instead. Please see guide TODO!",
        LinkCode = "brc-20-getbestvalue",
    };



    /// <summary>
    /// List re-used in v12 and v14
    /// </summary>
    [field: AllowNull, MaybeNull]
    internal static List<CodeHelp> Issues12To14 => field ??=
    [
        ..IssuesRazor12AlsoForRuntime,
        ..BuildListFromDiverseSources(
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
                Detect =
                    @"error CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'ToSic.Eav' (are you missing an assembly reference?)",
                UiMessage =
                    "You are probably using the old namespace ToSic.Eav.Interfaces, which is not supported since v20. Replace \"ToSic.Eav.Interfaces\" with \"ToSic.Eav.Data\" in your code.",
                LinkCode = "ErrIEntity",
            },
            new CodeHelp
            {
                Name = "Using-ToSic.SexyContent.Interfaces",
                Detect =
                    @"error CS0234: The type or namespace name 'Interfaces' does not exist in the namespace 'ToSic.SexyContent' (are you missing an assembly reference?)",
                UiMessage =
                    "You are probably using the old namespace ToSic.SexyContent.Interfaces, which is not supported since v20. Please remove/replace according to upgrade guide TODO!.",
            },

            // New v20 - detect usage of `IEntity` without the namespace 
            new CodeHelp
            {
                Name = "IEntity-Without-Namespace",
                Detect =
                    @"error CS0246: The type or namespace name 'IEntity' could not be found (are you missing a using directive or an assembly reference?)",
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
                UiMessage =
                    "Your code seems to use an old interface IEntityLight. Best just use 'ToSic.Eav.Data.IEntity' or see if the conversion is even necessary",
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

            GetBestValueGone,

            new CodeHelp
            {
                Name = "Detect use of Get<T> without using",
                // Full error is something like: "error CS0308: The non-generic method 'IEntity.Get(string)' cannot be used with type arguments"
                Detect = @"error CS0308: The non-generic method 'IEntity.Get(string)' cannot be used with type arguments",
                UiMessage = "Your code seems to use a generic 'Get...(...). This is an extension method as of v20. Make sure you add '@using ToSic.Eav.Data' to your razor file.",
            },

            HelpForRazorCompileErrors.BlocksIRenderServiceRemoved,

            #endregion

            // Added in v20, but for everything - detect calls to APIs without using parameter names
            new CodeHelp
            {
                Name = "Detect missing parameter names",
                // Full error is something like: "error CS1503: Argument 2: cannot convert from 'string' to 'ToSic.Lib.Coding.NoParamOrder' at System.Web.Compilation.AssemblyBuilder.Compile()"
                // It seems that the ' may be escaped, so we're really trying to just detect the bare minimum
                Detect = @"ToSic.Lib.Coding.NoParamOrder",
                UiMessage =
                    "Your code seems to call an API which expects named parameters, and you didn't name them. See help.",
                LinkCode = "https://go.2sxc.org/named-params",
            },

            // .CreateSource(string) - Obsolete
            new CodeHelp
            {
                Name = "CreateSource-String-Obsolete",
                Detect =
                    @"error CS0411: The type arguments for method .*\.CreateSource.*cannot be inferred from the usage",
                DetectRegex = true,
                LinkCode =
                    "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.Services.IDataService.html#ToSic_Sxc_Services_IDataService_GetSource_",
                UiMessage = $"""

                             You are probably calling CreateSource(stringNameOfSource, ...) which {IsNotSupportedIn12Plus}. 

                             """,
                DetailsHtml = $"""

                               You are probably calling <code>CreateSource(stringNameOfSource, ...)</code> which {IsNotSupportedIn12Plus}. Use: 
                               <ol>
                                   <li>Kit.Data.GetSource&lt;TypeName&gt;(...)</li>
                                   <li>Kit.Data.GetSource(appDataSourceName, ...)</li>
                               </ol>

                               """
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
        )
    ];

    /// <summary>
    /// All issues for v12 - ATM identical with the shared list.
    /// </summary>
    internal static List<CodeHelp> Compile12 => Issues12To14;


    #region Help which is used in various places

    internal static CodeHelp DnnObjectNotInHybrid = new()
    {
        Name = "Object-Dnn-Not-In-Hybrid",
        Detect = @"error CS0118: 'Dnn' is a 'namespace' but is used like a 'variable'",
        UiMessage = $"""
                     You are probably trying to use the 'Dnn' object which is not supported in 'Custom.Hybrid.Razor' templates. 

                     """,
        DetailsHtml = $"""
                       You are probably trying to use the <code>Dnn</code> object which is not supported in <code>Custom.Hybrid.Razor</code> templates. Use: 
                       <ol>
                           <li>Other APIs such as <code>CmsContext</code> to get page/module etc. information</li>
                           <li>If really necessary (not recommended) use the standard Dnn APIs to get the necessary objects.</li>
                       </ol>

                       """
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