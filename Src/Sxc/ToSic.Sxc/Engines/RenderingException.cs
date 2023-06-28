using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Code.Help;

namespace ToSic.Sxc.Engines
{
    public class RenderingException: Exception, IExceptionWithHelp
    {
        public RenderingException(CodeHelp help, string message = default) : base(message ?? help.UiMessage)
        {
            Helps = new List<CodeHelp> { help };
        }
        

        public RenderingException(CodeHelp help, Exception inner) : base("Rendering Exception", inner)
        {
            Helps = new List<CodeHelp> { help };
        }

        public List<CodeHelp> Helps { get; }
    }
}