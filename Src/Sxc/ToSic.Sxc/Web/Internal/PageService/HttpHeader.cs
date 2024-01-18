namespace ToSic.Sxc.Web.Internal.PageService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HttpHeader(string name, string value)
{
    public string Name { get; set; } = name;
    public string Value { get; set; } = value;

    /// <summary>
    /// If set, would flush existing headers
    /// </summary>
    public bool Reset { get; set; }
}