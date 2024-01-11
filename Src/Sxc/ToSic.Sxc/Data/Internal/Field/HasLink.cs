namespace ToSic.Sxc.Data.Internal;

/// <summary>
/// Special helper object pass around a url when it started as a string
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HasLink: IHasLink
{
    internal HasLink(string url) => Url = url;

    public string Url { get; }
}