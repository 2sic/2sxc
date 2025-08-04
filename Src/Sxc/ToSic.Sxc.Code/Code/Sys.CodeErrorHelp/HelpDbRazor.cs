using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Sys.CodeErrorHelp;

/// <summary>
/// This class contains a bunch of lists of various error detections and corresponding help.
/// </summary>
/// <remarks>
/// It is quite challenging to manage the help, to ensure that the lists match the needs.
/// So make sure you follow these conventions:
///
/// 1. Only the first / main file `HelpDbRazor` can contain lists which are shared (internal)
/// 2. All other parts of this class MUST contain private! lists, so we know they are not used outside of this
/// 3. Clearly separate between lists which contain the definitions, and lists which merge them for a specific use.
/// 4. Never stack partial lists in lists, it makes it very hard to see where a help is being used
/// 5. But stacking these lists is allowed for consistency (in this file only)
/// </remarks>
public partial class HelpDbRazor
{
    /// <summary>
    /// All issues for v12 Razor only
    /// </summary>
    internal static List<CodeHelp> CompileRazorOrCode12 =>
    [
        ..DnnMissingInHybrid,
        ..RemovedV20,
        ..TryingToUseV8ApiIn12Plus,
    ];


    [field: AllowNull, MaybeNull]
    internal static List<CodeHelp> CompileRazorOrCode14 => field ??=
    [
        ..DnnMissingInHybrid,
        ..RemovedV20,

        // use `Convert`
        ..ChangesInV14LikeConvert,

        ..TryingToUseV16InV14AsItWasTemporarilyMixed,

    ];

    /// <summary>
    /// Compile Help for RazorTyped etc.
    /// </summary>
    [field: AllowNull, MaybeNull]
    public static List<CodeHelp> Compile16 => field ??=
    [
        ..DnnMissingInHybrid,
        ..RemovedV20,

        // use old `Convert` object
        ..ChangesInV14LikeConvert,

        ..Compile16Wip,

        ..CompileV16AndOthers,

        // razor compile errors
        ..CompileUnknownOnly,

        // New v20
        ..CompileRemovedApisInV20ForAllRazorClasses,
    ];


    [field: AllowNull, MaybeNull]
    public static List<CodeHelp> HelpForRuntimeProblems => field ??=
    [
        ..RemovedV12,
        ..RemovedV20,
        ..RuntimeProblemsOnly,
    ];


    [field: AllowNull, MaybeNull]
    internal static List<CodeHelp> CompileVersionUnknown => field ??=
    [
        // razor compile errors where the file type isn't known
        ..CompileUnknownOnly,

        ..CompileRemovedApisInV20ForAllRazorClasses,
        
        ..CompilerErrorsWhenOldAutomaticWebConfigIsMissing,

        ..RemovedV20,
    ];

    [field: AllowNull, MaybeNull]
    public static List<CodeHelp> HelpForHttpCompileExceptions => field ??=
    [
        // Inherited from above
        ..CompileVersionUnknown,

        ..RemovedV12,
        ..CompileProblemsGeneral,
        ..CompileProblemsDynamic,
    ];

    [field: AllowNull, MaybeNull]
    public static List<CodeHelp> CompileInvalidCastExceptions => field ??=
    [
        ..RemovedV12,
    ];


    /// <summary>
    /// This is used directly in the DNN code when the base class doesn't match what's expected.
    /// Not reused anywhere else.
    /// </summary>
    [field: AllowNull, MaybeNull]
    public static CodeHelp AutoInheritsMissingAfterV20 => field ??= new()
    {
        Name = "auto-inherits-missing-after-v20",
        Detect = "The webpage at",
        UiMessage = """

                    In v20, auto inheritance is removed. Make sure your template inherits from the correct base class and check the docs for updated usage.

                    """,
        DetailsHtml = """

                      This is likely because your template is missing the correct <code>@inherits</code> statement.
                      <br>
                      <strong>Solution:</strong> <br>
                      Ensure your Razor file has the correct <code>@inherits</code> statement for v20. See the docs for more info.
                      <br>
                      <code>@inherits ToSic.SexyContent.Razor.SexyContentWebPage</code>
                      <br>
                      See the docs for more info.

                      """,
        LinkCode = "brc-20-stop-auto-inherits",
    };

}
