using ToSic.Sys.Code.Help;
using static ToSic.Sxc.Code.Sys.CodeErrorHelp.CodeHelpBuilder;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class HelpDbRazor
{
    #region Methods to build Help DB

    internal const string IsNotSupportedIn12Plus = "is not supported in Razor12+";

    private static CodeHelp HelpNotExists12(string property, params string[] replacement)
        => new GenNotExist(property, replacement)
        {
            MsgNotSupportedIn = IsNotSupportedIn12Plus
        }.Generate();

    #endregion

    [field: AllowNull, MaybeNull]
    public static List<CodeHelp> RemovedV12 => field ??=
    [
        new()
        {
            // Very old use of ToSic.Eav.IEntity
            Name = "ToSic.Eav.IEntity",
            Detect = "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
            LinkCode = "ErrIEntity",
            UiMessage = "IEntity is used on the wrong namespace, correct would be ToSic.Eav.Data.IEntity."
        },
    ];


    /// <summary>
    /// List re-used in v12 and v14
    /// </summary>
    [field: AllowNull, MaybeNull]
    private static List<CodeHelp> TryingToUseV8ApiIn12Plus => field ??=
    [
        // Access .List
        ..BuildVariations(
            HelpNotExists12("List", "AsDynamic(Data)"),
            h => h with
            {
                Detect = "does not contain a definition for 'List'",
            },
            h => h with
            {
                Detect =
                @"error CS0305: Using the generic type 'System.Collections.Generic.List<T>' requires 1 type arguments",
            }
        ),

        // Access .ListContent
        HelpNotExists12("ListContent", "Header"),
        HelpNotExists12("ListPresentation", "Header.Presentation"),

        // .Presentation
        HelpNotExists12("Presentation", "Content.Presentation"),

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
    ];
}