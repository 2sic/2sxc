using System.Collections.Generic;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Code.Help
{
    internal class CodeHelpList
    {
        public static CodeHelp IEntityOnEavNamespace = new CodeHelp(name: "ToSic.Eav.IEntity",
            detect: "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
            linkCode: "ErrIEntity",
            uiMessage: "IEntity is used on the wrong namespace.");

        public static CodeHelp DynamicList = new CodeHelp(name: "Can't use Lambda",
            detect: "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
            linkCode: "ErrLambda",
            uiMessage: "Lambdas have difficulties with dynamic objects.");

        public static CodeHelp DynamicEntity = new CodeHelp(name: "DynamicEntity not found",
            detect: "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
            linkCode: "ErrDynamicEntity", 
            uiMessage: "The type DynamicEntity shouldn't be used.");

        public static CodeHelp MethodOnObjectNotFound = new CodeHelp(name: "Method on Object not found",
            detect: "Cannot perform runtime binding on a null reference",
            linkCode: "err-binding-on-null-reference",
            uiMessage: "Method not found - typically on a dynamic object.");

        public static List<CodeHelp> ListInvalidCast = new List<CodeHelp>
        {
            IEntityOnEavNamespace
        };

        public static List<CodeHelp> ListHttpCompile = new List<CodeHelp>(ListInvalidCast)
        {
            DynamicList,
            DynamicEntity,
        };

        public static List<CodeHelp> ListRuntime = new List<CodeHelp>(ListInvalidCast)
        {
            MethodOnObjectNotFound,
        };
    }

}
