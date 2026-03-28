using ToSic.Sxc.Services.Sys;

// Important: the namespace cannot be "HttpContext" because that would cause a conflict with the System.Web.HttpContext class in .net framework
// which is used in the implementation. So we use "HttpCtx" instead.
namespace ToSic.Sxc.Services.HttpCtx;

internal class HttpContextService(): ServiceWithContext("Sxc.HttpCx"), IHttpContextService
{
    public void Redirect301(string url) => Redirect(url, 301);

    public void Redirect302(string url) => Redirect(url, 302);


    public void Redirect(string url, int statusCode)
    {
#if NETFRAMEWORK
        // The .net framework implementation uses the "global" HttpContext, which isn't perfect, but that's how it works in DNN.
        var response = System.Web.HttpContext.Current.Response;

        // Clear any previous headers/content
        response.Clear();

        // Manually set the 301 status code - defaults to 302 if not specified
        response.StatusCode = statusCode;
        response.StatusDescription = StatusDescriptions.TryGetValue(statusCode, out var description)
            ? description
            : "Unknown Status";

        // Set the destination
        response.AddHeader("Location", url);

        // End the response to prevent further processing
        response.End();
#else
        // TODO: @STV IMPLEMENT
#endif

    }

    /// <summary>
    /// 300-level status codes and their descriptions for reference. This can be expanded as needed.
    /// https://developer.att.com/video-optimizer/docs/best-practices/http-300-status-codes
    /// </summary>
    private static readonly Dictionary<int, string> StatusDescriptions = new()
    {
        { 301, "Moved Permanently" },
        { 302, "Found" },
        { 303, "See Other" },
        { 304, "Not Modified" },
        { 305, "Use Proxy" },
        { 306, "obsolete status code - not used" },
        { 307, "Temporary Redirect" },
        { 308, "Permanent Redirect" }
    };
}
