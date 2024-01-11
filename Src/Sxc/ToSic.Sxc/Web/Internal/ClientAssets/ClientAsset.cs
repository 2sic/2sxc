namespace ToSic.Sxc.Web.Internal.ClientAssets;

internal class ClientAsset : IClientAsset
{
    /// <inheritdoc />
    public string Id { get; set; }
        
    public bool IsJs { get; set; }= true;
    public string Url { get; set; }
    public int Priority {get; set; }
    public string PosInPage { get; set; } = "body";
    //public bool AutoOpt { get; set; } = false;

    public bool IsExternal { get; set; } = true;
    public string Content { get; set; } = null;

    public bool WhitelistInCsp { get; set; } = false;

    /// <inheritdoc />
    public IDictionary<string, string> HtmlAttributes { get; set; } = null;
}