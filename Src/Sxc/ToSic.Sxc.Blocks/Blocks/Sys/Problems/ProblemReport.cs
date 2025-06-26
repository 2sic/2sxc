using System.Text.Json.Serialization;
using static System.Text.Json.Serialization.JsonIgnoreCondition;

namespace ToSic.Sxc.Blocks.Sys.Problems;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ProblemReport
{
    // ReSharper disable InconsistentNaming
    public enum ErrorSeverity
    {
        none = 0,
        info = 1,
        warning = 2,
        error = 3,
    }

    public enum ErrorScope
    {
        view,
        app
    }
    // ReSharper restore InconsistentNaming

    [JsonPropertyName("severity")]
    public ErrorSeverity Severity { get; set; }

    [JsonPropertyName("scope")]
    public ErrorScope Scope { get; set; }

    [JsonPropertyName("code")]
    [JsonIgnore(Condition = WhenWritingDefault)]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }
}