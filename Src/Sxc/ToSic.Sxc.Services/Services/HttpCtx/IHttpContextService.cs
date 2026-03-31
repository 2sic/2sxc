#pragma warning disable IDE0130
namespace ToSic.Sxc.Services;
#pragma warning restore IDE0130

/// <summary>
/// Internal service for now, not for public use.
/// Later we'll probably create a different API to make this more generic for .net framework and .net core,
/// and it will probably not be called HttpContext.
/// </summary>
[PrivateApi("WIP 21.06, not for public use, internal only")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IHttpContextService
{
    /// <summary>
    /// Do a redirect to a url with a custom status code.
    /// </summary>
    /// <remarks>
    /// This is a low-level method, and the status code should be a 300-level code for redirection.
    /// For common use cases, consider using the Redirect301 or Redirect302 methods which are more straightforward and set the appropriate status codes automatically.
    /// </remarks>
    /// <param name="url">url to redirect to.</param>
    /// <param name="statusCode">The HTTP status code to use for the redirect. Cannot be null or empty.</param>
    void Redirect(string url, int statusCode);

    /// <summary>
    /// Performs a permanent HTTP 301 redirect to the specified URL.
    /// </summary>
    /// <remarks>
    /// Use this method to indicate that a resource has been permanently moved to a new location.
    /// Clients and search engines will update their references to the new URL. Ensure that the specified URL is valid
    /// and accessible.
    /// </remarks>
    /// <param name="url">The destination URL to which the client is permanently redirected. Cannot be null or empty.</param>
    void Redirect301(string url);

    /// <summary>
    /// Performs a temporary HTTP 302 redirect to the specified URL.
    /// </summary>
    /// <remarks>
    /// Use this method to indicate that a resource has been temporarily moved to a new location.
    /// Clients and search engines will continue to use the original URL for future requests.
    /// </remarks>
    /// <param name="url">The destination URL to which the client is temporarily redirected. Cannot be null or empty.</param>
    void Redirect302(string url);
}