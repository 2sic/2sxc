using System.Collections.Generic;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Help;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeHelpList
{
    public static CodeHelp IEntityOnEavNamespace = new(name: "ToSic.Eav.IEntity",
        detect: "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
        linkCode: "ErrIEntity",
        uiMessage: "IEntity is used on the wrong namespace.");

    public static CodeHelp DynamicList = new(name: "Can't use Lambda",
        detect: "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
        linkCode: "ErrLambda",
        uiMessage: "Lambdas have difficulties with dynamic objects.");

    public static CodeHelp DynamicEntity = new(name: "DynamicEntity not found",
        detect: "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
        linkCode: "ErrDynamicEntity", 
        uiMessage: "The type DynamicEntity shouldn't be used.");

    public static CodeHelp MethodOnObjectNotFound = new(name: "Method on Object not found",
        detect: "Cannot perform runtime binding on a null reference",
        linkCode: "err-binding-on-null-reference",
        uiMessage: "Method not found - typically on a dynamic object.");

    public static List<CodeHelp> ListInvalidCast = new()
    {
        IEntityOnEavNamespace
    };

    public static List<CodeHelp> ListHttpCompile = new(ListInvalidCast)
    {
        DynamicList,
        DynamicEntity,
    };

    public static List<CodeHelp> ListRuntime = new(ListInvalidCast)
    {
        MethodOnObjectNotFound,
    };
}