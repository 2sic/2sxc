using System;

namespace ToSic.SexyContent
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