using System;
using System.Collections.Generic;
using ToSic.Eav.Plumbing;

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

        public static List<CodeError> InvalidCastExceptions = new List<CodeError>
        {
            IEntityOnEavNamespace
        };

        public static List<CodeError> HttpCompileExceptions = new List<CodeError>(InvalidCastExceptions)
        {
            DynamicList,
            DynamicEntity,
        };

        public static string FindAdditionalText(Exception ex, List<CodeError> errorList)
        {
            var msg = ex?.Message;
            if (msg == null) return null;
            foreach (var help in errorList)
                if (msg.Contains(help.Detect))
                    return help.ShowMessage;

            return null;
        }
    }


    internal readonly struct CodeError
    {
        public const string ErrHelpPre = "Error in your code. ";// "***** Probably https://go.2sxc.org/";
        public const string ErrHelpLink = "***** Probably https://go.2sxc.org/{0} can help! ***** \n";
        public const string ErrHelpSuf = /*" can help! ***** \n " +*/ "What follows is the internal error: ";

        public CodeError(string name, string detect, string linkCode, string message = null)
        {
            Name = name;
            Detect = detect;
            _message = message;
            _linkCode = linkCode;
        }
        /// <summary>
        /// Name for internal use to better understand what this is for
        /// </summary>
        public readonly string Name;
        public readonly string Detect;
        private readonly string _message;
        private readonly string _linkCode;
        private string Link => _linkCode.HasValue() ? string.Format(ErrHelpLink, _linkCode) : "";

        public string ShowMessage => $"{ErrHelpPre} {_message} {Link} {ErrHelpSuf}";
    }
}
