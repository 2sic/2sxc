using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// For the item-swap dialog.
/// Contains brief info about the current item and the items which could be used as replacements.
/// </summary>
public record ListSlotSwapInfo
{
    [JsonPropertyName("selectedId")]
    public int? SelectedId { get; init; }

    [JsonPropertyName("items")]
    public required ICollection<ListSlotSwapCandidate> Items { get; init; }
}
