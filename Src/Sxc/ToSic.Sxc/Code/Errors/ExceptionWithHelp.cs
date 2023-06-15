using System;

namespace ToSic.Sxc.Code.Errors
{
    public class ExceptionWithHelp: Exception, IExceptionWithHelp
    {
        internal ExceptionWithHelp(CodeError help, Exception inner = null) : base(help.ErrorMessage, inner)
        {
            Help = help;
        }

        public CodeError Help { get; }

        public ExceptionWithHelp(string message, Exception inner): base(message, inner)
        { }
    }
}
