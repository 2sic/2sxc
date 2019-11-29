using System;

namespace ToSic.Sxc.Engines
{
    public class SexyContentException : Exception
    {
        public SexyContentException(string Message)
            : base(Message)
        {

        }
        public bool IsOnlyMessage = false;
    }
}