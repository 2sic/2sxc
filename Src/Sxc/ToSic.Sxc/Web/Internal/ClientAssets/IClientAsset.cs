namespace ToSic.Sxc.Web.Internal.ClientAssets;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IClientAsset
{
    /// <summary>
    /// Asset ID for use in HTML - ideally should ensure that this asset is only loaded once
    /// </summary>
    string Id { get; set; }

    bool IsJs { get; set; }
    string Url { get; set; }
    int Priority { get; set; }
    string PosInPage { get; set; }
    //bool AutoOpt { get; set; }
    bool IsExternal { get; set; }
    string Content { get; set; }

    bool WhitelistInCsp { get; set; }


    /// <summary>
    /// Used to store all other html attributes from html tag.
    /// </summary>
    IDictionary<string, string> HtmlAttributes { get; set; }
}