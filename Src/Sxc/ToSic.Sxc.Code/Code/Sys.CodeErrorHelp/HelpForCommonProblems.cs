using ToSic.Sys.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class HelpForCommonProblems
{
    private static readonly CodeHelp IEntityOnEavNamespace = new()
    {
        Name = "ToSic.Eav.IEntity",
        Detect = "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
        LinkCode = "ErrIEntity",
        UiMessage = "IEntity is used on the wrong namespace, correct would be ToSic.Eav.Data.IEntity."
    };

    private static readonly CodeHelp DynamicList = new()
    {
        Name = "Can't use Lambda",
        Detect = "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
        LinkCode = "ErrLambda",
        UiMessage = "Lambdas have difficulties with dynamic objects."
    };

    private static readonly CodeHelp DynamicEntity = new()
    {
        Name = "DynamicEntity not found",
        Detect = "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
        LinkCode = "ErrDynamicEntity",
        UiMessage = "The type DynamicEntity shouldn't be used.",
    };

    private static readonly CodeHelp MethodOnObjectNotFound = new()
    {
        Name = "Method on Object not found",
        Detect = "Cannot perform runtime binding on a null reference",
        LinkCode = "err-binding-on-null-reference",
        UiMessage = "Method not found - typically on a dynamic object.",
    };

    private static readonly CodeHelp NoParamOrderUsed = new()
    {
        Name = "Advanced APIs should use Parameter-Names and not Param-Order",
        // real error is ca. :error CS1503: Argument 2: cannot convert from 'bool' to 'ToSic.Lib.Coding.NoParamOrder' at System.Web.Compilation.AssemblyBuilder.Compile()
        Detect = "to 'ToSic.Lib.Coding.NoParamOrder'",
        LinkCode = "named-params",
        UiMessage = "Many methods have optional parameters - these must be named, otherwise you see this error."
    };

    public static List<CodeHelp> HelpForInvalidCast =
    [
        IEntityOnEavNamespace
    ];

    public static List<CodeHelp> HelpForHttpCompile =
    [
        ..HelpForInvalidCast,
        DynamicList,
        DynamicEntity,
        NoParamOrderUsed,
    ];

    public static List<CodeHelp> HelpForRuntimeProblems =
    [
        ..HelpForInvalidCast,
        ..HelpForRazor12.IssuesRazor12AlsoForRuntime,
        MethodOnObjectNotFound
    ];
}