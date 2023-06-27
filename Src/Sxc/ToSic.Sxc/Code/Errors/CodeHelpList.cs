using System.Collections.Generic;

namespace ToSic.Sxc.Code.Errors
{
    internal class CodeHelpList
    {
        public static CodeHelp IEntityOnEavNamespace = new CodeHelp("ToSic.Eav.IEntity",
            "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
            "ErrIEntity",
            "IEntity is used on the wrong namespace.");

        public static CodeHelp DynamicList = new CodeHelp("Can't use Lambda",
            "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
            "ErrLambda",
            "Lambdas have difficulties with dynamic objects.");

        public static CodeHelp DynamicEntity = new CodeHelp("DynamicEntity not found",
            "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
            "ErrDynamicEntity", 
            "The type DynamicEntity shouldn't be used.");

        public static CodeHelp MethodOnObjectNotFound = new CodeHelp("Method on Object not found",
            "Cannot perform runtime binding on a null reference",
            "err-binding-on-null-reference",
            "Method not found - typically on a dynamic object.");

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
