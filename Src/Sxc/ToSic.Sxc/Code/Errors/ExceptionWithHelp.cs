using System;

namespace ToSic.Sxc.Code.Errors
{
    public class ExceptionWithHelp: Exception
    {
        public ExceptionWithHelp(string message, Exception inner): base(message, inner)
        { }
    }
}
