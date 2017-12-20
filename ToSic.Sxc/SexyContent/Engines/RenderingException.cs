using System;

namespace ToSic.SexyContent.Engines
{
    public class RenderingException: Exception
    {
        public RenderStatusType RenderStatus = RenderStatusType.Error;
        public bool ShouldLog => RenderStatus != RenderStatusType.Ok;

        public RenderingException(string message) : base(message) { }
        public RenderingException(string message, Exception innerException) : base(message, innerException) { }
        public RenderingException(Exception innerException): base("Rendering Exception", innerException) { }

        public RenderingException(RenderStatusType renderStat, string message): base(message)
        {
            RenderStatus = renderStat;
        }

        public RenderingException(RenderStatusType renderStat, Exception innerException)
            : base("Rendering Message", innerException)
        {
            RenderStatus = renderStat;
        }
    }
}