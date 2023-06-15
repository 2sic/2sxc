using System;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Code.Errors
{
    internal class CodeErrorsList
    {
        public static CodeError IEntityOnEavNamespace = new CodeError("ToSic.Eav.IEntity",
            "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
            "ErrIEntity",
            "IEntity is used on the wrong namespace.");

        public static CodeError DynamicList = new CodeError("Can't use Lambda",
            "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
            "ErrLambda",
            "Lambdas have difficulties with dynamic objects.");

        public static CodeError DynamicEntity = new CodeError("DynamicEntity not found",
            "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
            "ErrDynamicEntity", 
            "The type DynamicEntity shouldn't be used.");

        public static CodeError MethodOnObjectNotFound = new CodeError("Method on Object not found",
            "Cannot perform runtime binding on a null reference",
            "err-binding-on-null-reference",
            "Method not found - typically on a dynamic object.");

        public static List<CodeError> ListInvalidCast = new List<CodeError>
        {
            IEntityOnEavNamespace
        };

        public static List<CodeError> ListHttpCompile = new List<CodeError>(ListInvalidCast)
        {
            DynamicList,
            DynamicEntity,
        };

        public static List<CodeError> ListRuntime = new List<CodeError>(ListInvalidCast)
        {
            MethodOnObjectNotFound,
        };

        public static CodeError FindHelp(Exception ex, List<CodeError> errorList)
        {
            var msg = ex?.Message;
            return msg == null ? null : errorList.FirstOrDefault(help => msg.Contains(help.Detect));
        }
    }

}
