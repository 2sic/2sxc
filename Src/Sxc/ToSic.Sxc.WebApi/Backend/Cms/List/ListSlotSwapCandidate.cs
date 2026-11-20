using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// For the item-swap dialog.
/// Contains brief info about an entity which could be swapped out.
/// </summary>
/// <remarks>
/// When a list of entities (in a specific field of another entity) wants to let the user swap it with another one,
/// we need to provide it with basic info about the alternatives.
/// </remarks>
public record ListSlotSwapCandidate
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    [JsonPropertyName("contentType")]
    public required string ContentType { get; init; }
}
