using System;

namespace ToSic.Sxc.Code.Errors
{
    public class ExceptionWithHelp: Exception
    {
        internal ExceptionWithHelp(CodeError help, Exception inner) : base(help.Message, inner)
        {
            Help = help;
        }

        internal CodeError Help;

        public ExceptionWithHelp(string message, Exception inner): base(message, inner)
        { }
    }
}
