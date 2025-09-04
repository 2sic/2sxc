namespace ToSic.Sxc.Backend.InPage;

public class AjaxRenderDto
{
    public required string? Html { get; init; }

    public required IEnumerable<AjaxResourceDto> Resources { get; init; }
}

public record AjaxResourceDto
{
    public string? Url { get; init; }

    /// <summary>
    /// "js" or "css"
    /// </summary>
    public string Type { get; init; } = "js";

    public string? Contents { get; init; }

    public IDictionary<string, string>? Attributes { get; init; }
}