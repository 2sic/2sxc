namespace ToSic.Sxc.Web.Internal.ClientAssets;

public record ClientAsset
{
    /// <summary>
    /// Asset ID for use in HTML - ideally should ensure that this asset is only loaded once
    /// </summary>
    public string Id { get; init; }
        
    public bool IsJs { get; init; }= true;
    public string Url { get; init; }
    public int Priority {get; init; }
    public string PosInPage { get; init; } = "body";

    public bool IsExternal { get; init; } = true;
    public string Content { get; init; } = null;

    public bool WhitelistInCsp { get; init; } = false;

    /// <summary>
    /// Used to store all other html attributes from html tag.
    /// </summary>
    public IDictionary<string, string> HtmlAttributes { get; init; } = null;
}