using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class HelpForCommonProblems
{
    private static readonly CodeHelp IEntityOnEavNamespace = new(name: "ToSic.Eav.IEntity",
        detect: "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
        linkCode: "ErrIEntity",
        uiMessage: "IEntity is used on the wrong namespace, correct would be ToSic.Eav.Data.IEntity.");

    private static readonly CodeHelp DynamicList = new(name: "Can't use Lambda",
        detect: "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
        linkCode: "ErrLambda",
        uiMessage: "Lambdas have difficulties with dynamic objects.");

    private static readonly CodeHelp DynamicEntity = new(name: "DynamicEntity not found",
        detect: "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
        linkCode: "ErrDynamicEntity", 
        uiMessage: "The type DynamicEntity shouldn't be used.");

    private static readonly CodeHelp MethodOnObjectNotFound = new(name: "Method on Object not found",
        detect: "Cannot perform runtime binding on a null reference",
        linkCode: "err-binding-on-null-reference",
        uiMessage: "Method not found - typically on a dynamic object.");

    private static readonly CodeHelp NoParamOrderUsed = new(name: "Advanced APIs should use Parameter-Names and not Param-Order",
        // real error is ca. :error CS1503: Argument 2: cannot convert from 'bool' to 'ToSic.Lib.Coding.NoParamOrder' at System.Web.Compilation.AssemblyBuilder.Compile()
        detect: "to 'ToSic.Lib.Coding.NoParamOrder'",
        linkCode: "named-params",
        uiMessage: "Many methods have optional parameters - these must be named, otherwise you see this error.");

    public static List<CodeHelp> HelpForInvalidCast =
    [
        IEntityOnEavNamespace
    ];

    public static List<CodeHelp> HelpForHttpCompile =
    [
        ..HelpForInvalidCast,
        DynamicList,
        DynamicEntity,
        NoParamOrderUsed
    ];

    public static List<CodeHelp> HelpForRuntimeProblems =
    [
        ..HelpForInvalidCast,
        MethodOnObjectNotFound
    ];
}