using System.Text.Json.Serialization;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Apps.Internal.Assets;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TemplateInfo(
    string key,
    string name,
    string extension,
    string suggestedFileName,
    string purpose,
    string type)
{
    public string Key { get; } = key;

    public string Name { get; } = name;

    public string SuggestedFileName { get; } = suggestedFileName;

    public string Extension { get; set; } = extension;

    public string Purpose { get; set; } = purpose;

    public string Type { get; set; } = type;

    public string Folder { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Body { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Description { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Prefix { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Suffix { get; set; }

    /// <summary>
    /// Returns an array of platforms this template supports so the UI can pick
    /// </summary>
    public IEnumerable<string> Platforms => PlatformTypes?.ToString().Split(',').Select(p => p.Trim());

    [JsonIgnore]
    public PlatformType? PlatformTypes { get; set; } = PlatformType.Hybrid | PlatformType.Dnn | PlatformType.Oqtane;
}