using System.Text.Json.Serialization;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ErrorDto
    {
        [JsonPropertyName("type")]
        public string Type { get; }

        public string Message { get; }

        internal ErrorDto(IBlock cb)
        {
            if (cb.DataIsMissing)
                Type = "DataIsMissing";
        }
    }
}
