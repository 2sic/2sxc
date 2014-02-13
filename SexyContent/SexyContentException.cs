using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent
{
    public class SexyContentException : System.Exception
    {
        public SexyContentException(string Message)
            : base(Message)
        {

        }
        public bool IsOnlyMessage = false;
    }
}