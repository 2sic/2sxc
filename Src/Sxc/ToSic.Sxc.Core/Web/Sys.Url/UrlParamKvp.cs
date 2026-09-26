namespace ToSic.Sxc.Web.Sys.Url;

/// <summary>
/// Helper class to prepare data for use in a url parameter.
/// Especially useful to ensure that the value part is encoded, but not re-encoded.
/// </summary>
/// <remarks>
/// Very temp, just for inside ObjectToUrl.
/// Leaving it internal temporarily, as we'll probably try to better split the code...
/// </remarks>
internal sealed record UrlParamKvp(string? Name, string? Value): IUrlParam
{
    /// <summary>
    /// Converts the UrlParameter to a URL fragment string.
    /// </summary>
    /// <returns>A string representation of the URL parameter.</returns>
    public string GetSerialized()
    {
        var start = Name != null
            ? Name + "="
            : null;
        var val = Value == null
            ? null
            // Note: Uri.EscapeDataString encodes commas as %2C, so we replace them back to commas.
            // Previously we used Uri.EscapeUriString, but that has been deprecated. It left commas as-is.
            : Uri.EscapeDataString(Value).Replace("%2C", ",");
        return $"{start}{val}";
    }

}

internal record UrlParamPrepared(string Fragment): IUrlParam
{
    public string GetSerialized() => Fragment;
}