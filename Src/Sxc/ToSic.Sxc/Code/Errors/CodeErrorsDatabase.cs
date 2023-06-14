using System;
using System.Collections.Generic;

namespace ToSic.Sxc.Code.Errors
{
    internal class CodeErrorsDatabase
    {
        public static CodeError IEntityOnEavNamespace = new CodeError("ToSic.Eav.IEntity",
            "error CS0234: The type or namespace name 'IEntity' does not exist in the namespace 'ToSic.Eav",
            "ErrIEntity");

        public static CodeError DynamicList = new CodeError("Can't use Lambda",
            "error CS1977: Cannot use a lambda expression as an argument to a dynamically dispatched operation without first casting it to a delegate or expression tree type",
            "ErrLambda");

        public static CodeError DynamicEntity = new CodeError("DynamicEntity not found",
            "error CS0246: The type or namespace name 'DynamicEntity' could not be found",
            "ErrDynamicEntity");

        public static CodeError MethodOnObjectNotFound = new CodeError("Method on Object not found",
            "Cannot perform runtime binding on a null reference",
            "ErrBindingOnNullReference");

        public static List<CodeError> InvalidCastExceptions = new List<CodeError>
        {
            IEntityOnEavNamespace
        };

        public static List<CodeError> HttpCompileExceptions = new List<CodeError>(InvalidCastExceptions)
        {
            DynamicList,
            DynamicEntity,
        };

        public static List<CodeError> Runtime = new List<CodeError>(InvalidCastExceptions)
        {
            MethodOnObjectNotFound,
        };
        public static CodeError FindHelp(Exception ex, List<CodeError> errorList)
        {
            var msg = ex?.Message;
            if (msg == null) return null;
            foreach (var help in errorList)
                if (msg.Contains(help.Detect))
                    return help;

            return null;
        }
        public static string FindAdditionalText(Exception ex, List<CodeError> errorList) => FindHelp(ex, errorList)?.Message;
    }



}
