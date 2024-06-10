namespace ToSic.Sxc.Backend.InPage;

public class AjaxRenderDto
{
    public string Html { get; init; }

    public IEnumerable<AjaxResourceDto> Resources { get; init; }
}

public class AjaxResourceDto
{
    public string Id { get; init; }

    public string Url { get; init; }

    /// <summary>
    /// "js" or "css"
    /// </summary>
    public string Type { get; init; } = "js";

    public string Contents { get; init; }

    public IDictionary<string, string> Attributes { get; init; }
}