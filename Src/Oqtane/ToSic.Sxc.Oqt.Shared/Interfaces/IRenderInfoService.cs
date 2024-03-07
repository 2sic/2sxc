namespace ToSic.Sxc.Oqt.Shared.Interfaces
{
    /// <summary>
    /// Service for providing rendering information based on the current HTTP context and module render mode.
    /// </summary>
    public interface IRenderInfoService
    {

        /// <summary>
        /// Checks if the module render mode is static SSR (Server-Side Rendering).
        /// </summary>
        /// <param name="renderMode">The module render mode.</param>
        /// <returns>True if the render mode is static SSR, otherwise false.</returns>
        bool IsStaticSsr(string renderMode);

        /// <summary>
        /// Checks if Blazor Enhanced Navigation is enabled for static SSR.
        /// </summary>
        /// <param name="renderMode">The module render mode.</param>
        /// <returns>True if Blazor Enhanced Navigation is enabled, otherwise false.</returns>
        /// <remarks>
        /// Blazor Enhanced Navigation is a feature in .NET 8 that allows for progressively-enhanced navigation in multi-page apps.
        /// This is enabled when the app loads `blazor.web.js` and does not use an interactive Router. It works by intercepting 
        /// navigation within the base href URI space, loading content via `fetch` requests, and syncing the DOM.
        /// </remarks>
        bool IsBlazorEnhancedNav(string renderMode);

        /// <summary>
        /// Checks if the SSR Framing response header exists, indicating a fetch page request in Blazor Enhanced Navigation.
        /// </summary>
        /// <param name="renderMode">The module render mode.</param>
        /// <returns>
        /// True if the SSR Framing response header exists (indicating a fetch page request during Blazor Enhanced Navigation).
        /// False if it is a standard full page load in the browser.
        /// </returns>
        /// <remarks>
        /// This helps distinguish between a full page load (initial load) and a fetch page request (subsequent navigation within the app).
        /// This behavior occurs when Blazor Enhanced Navigation is in use for static SSR.
        /// </remarks>
        bool IsSsrFraming(string renderMode);
    }
}
