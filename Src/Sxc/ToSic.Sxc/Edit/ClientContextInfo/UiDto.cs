using System.Text.Json.Serialization;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class UiDto
    {
        public bool AutoToolbar { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Edition { get; }

        public UiDto(bool autoToolbar)
        {
            AutoToolbar = autoToolbar;
        }
    }
}
