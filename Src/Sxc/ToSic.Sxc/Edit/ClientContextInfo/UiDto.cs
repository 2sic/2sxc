using Newtonsoft.Json;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class UiDto
    {
        public bool AutoToolbar { get; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string Edition { get; }

        public UiDto(bool autoToolbar)
        {
            AutoToolbar = autoToolbar;
        }
    }
}
