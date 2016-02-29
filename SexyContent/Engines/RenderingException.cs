using System;

namespace ToSic.SexyContent.Engines
{
    public class RenderingException: Exception
    {
        public bool IsOnlyMessage = false;
        public bool ShouldLog => !IsOnlyMessage;

        public RenderingException(string message) : base(message) { }
        public RenderingException(string message, Exception innerException) : base(message, innerException) { }
        public RenderingException(Exception innerException): base("Rendering Exception", innerException) { }

        public RenderingException(bool onlyMessage, string message): base(message)
        {
            IsOnlyMessage = onlyMessage;
        }

        public RenderingException(bool onlyMessage, Exception innerException)
            : base("Rendering Message", innerException)
        {
            IsOnlyMessage = onlyMessage;
        }
    }
}