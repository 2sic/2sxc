using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.Web.Internal.JsContextEdit;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class UiDto(bool autoToolbar)
{
    public bool AutoToolbar { get; } = autoToolbar;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Edition { get; }
}