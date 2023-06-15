using System;

namespace ToSic.Sxc.Code.Errors
{
    public class ExceptionWithHelp: Exception, IExceptionWithHelp
    {
        internal ExceptionWithHelp(CodeHelp help, Exception inner = null) : base(help.ErrorMessage, inner)
        {
            Help = help;
        }

        public CodeHelp Help { get; }

        public ExceptionWithHelp(string message, Exception inner): base(message, inner)
        { }
    }
}
