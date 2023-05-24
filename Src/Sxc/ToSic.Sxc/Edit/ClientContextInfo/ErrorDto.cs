using System.Text.Json.Serialization;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ErrorDto
    {
        [JsonPropertyName("type")]
        public string Type { get; }

        public string Message { get; }

        internal ErrorDto(IBlock cb, string errorCode)
        {
            // New mechanism in 16.01
            if (errorCode != null)
            {
                Type = errorCode;
                return;
            }

            // should probably be removed, if new mechanism works, because it should already set the error code into the string
            if (cb.DataIsMissing)
                Type = "DataIsMissing";
        }
    }
}
