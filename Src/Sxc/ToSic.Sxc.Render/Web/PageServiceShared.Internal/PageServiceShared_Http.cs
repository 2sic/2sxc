namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageServiceShared
{
    public int? HttpStatusCode { get; set; } = null;

    public string HttpStatusMessage { get; set; } = null;


    #region Headers WIP / BETA

    [PrivateApi]
    internal void AddToHttp(string name, string value) =>
        HttpHeaders.Add(new(name, value));

    [PrivateApi]
    public List<HttpHeader> HttpHeaders { get; } = [];

    #endregion

}