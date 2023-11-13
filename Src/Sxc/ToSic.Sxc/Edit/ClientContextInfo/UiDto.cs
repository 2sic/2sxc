using System.Text.Json.Serialization;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
