namespace ToSic.Sxc.Web.Sys.Url;

/// <summary>
/// Helper class to prepare data for use in a url parameter.
/// Especially useful to ensure that the value part is encoded, but not re-encoded.
/// </summary>
internal class UrlValuePair(string? name, string? value, bool isEncoded = false)
{
    public string? Name { get; } = name;
    public string? Value { get; } = value;
    public bool IsEncoded { get; } = isEncoded;

    public override string ToString()
    {
        var start = Name != null
            ? Name + "="
            : null;
        var val = IsEncoded
            ? Value
            : Value == null
                ? null
                // Note: Uri.EscapeDataString encodes commas as %2C, so we replace them back to commas.
                // Previously we used Uri.EscapeUriString, but that has been deprecated. It left commas as-is.
                : Uri.EscapeDataString(Value).Replace("%2C", ",");
        return $"{start}{val}";
    }

}