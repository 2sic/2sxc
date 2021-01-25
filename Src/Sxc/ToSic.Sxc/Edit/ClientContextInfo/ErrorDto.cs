using Newtonsoft.Json;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ErrorDto
    {
        [JsonProperty("type")]
        public string Type { get; }

        internal ErrorDto(IBlock cb)
        {
            if (cb.DataIsMissing)
                Type = "DataIsMissing";
        }
    }
}
