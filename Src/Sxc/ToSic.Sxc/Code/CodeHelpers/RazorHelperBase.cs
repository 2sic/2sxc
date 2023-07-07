using System;
using System.Collections.Generic;

namespace ToSic.Sxc.Code.CodeHelpers
{
    public abstract class RazorHelperBase: CodeHelperBase
    {
        protected RazorHelperBase(string logName) : base(logName)
        {

        }


        public List<Exception> ExceptionsOrNull { get; private set; }

        public void Add(Exception ex) => (ExceptionsOrNull ?? (ExceptionsOrNull = new List<Exception>())).Add(ex);

    }
}
