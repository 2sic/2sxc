using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Web.Sys.PageServiceShared;

partial class PageServiceShared
{
    public int? HttpStatusCode { get; set; } = null;

    public string? HttpStatusMessage { get; set; } = null;


    #region Headers WIP / BETA

    [PrivateApi]
    internal void AddToHttp(string name, string value) =>
        HttpHeaders.Add(new(name, value));

    /// <summary>
    /// Must be a real List, since it will be modified.
    /// </summary>
    [PrivateApi]
    public List<HttpHeader> HttpHeaders { get; } = [];

    #endregion

}