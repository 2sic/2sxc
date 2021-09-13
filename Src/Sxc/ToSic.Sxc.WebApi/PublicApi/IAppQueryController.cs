using System.Collections.Generic;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi.PublicApi
{
    /// <summary>
    /// In charge of delivering Pipeline-Queries on the fly
    /// They will only be delivered if the security is confirmed - it must be publicly available
    /// </summary>
    /// <remarks>
    /// The route for this is usually [some-base]/app/[app-name-or-auto]/query/[query-name]
    /// </remarks>
    public interface IAppQueryController
    {
        /// <summary>
        /// GET a public query by name, with some minimal options.
        /// Assumes that the context is given in the app-headers.
        /// </summary>
        /// <param name="name">Query name - ideally without spaces or special characters (required)</param>
        /// <param name="more">Additional parameters / filters etc. which would not fit into the url</param>
        /// <param name="includeGuid">Include GUID IDs of the items retrieved (optional)</param>
        /// <param name="stream">Stream names - leave empty or * for all. Comma separated if multiple. (optional)</param>
        /// <param name="appId">AppId to use in case we want to specify an app (optional)</param>
        /// <remarks>
        /// will check security internally, so assume the endpoint doesn't need to check security first
        /// </remarks>
        /// <returns></returns>
        IDictionary<string, IEnumerable<EavLightEntity>> Query(
            string name, 
            AppQueryParameters more,
            bool includeGuid = false, 
            string stream = null, 
            int? appId = null
            );

        /// <summary>
        /// GET a public query from an app in a specific path.
        /// The App-Path is for identifying the app itself, and is usually in the middle of the route.
        /// Example: [root]/app/[appPath]/query/[name]
        /// </summary>
        /// <param name="appPath">app-folder name (required)</param>
        /// <param name="name">Query name - ideally without spaces or special characters (required)</param>
        /// <param name="more">Additional parameters / filters etc. which would not fit into the url</param>
        /// <param name="stream">Stream names - leave empty or * for all. Comma separated if multiple. (optional)</param>
        /// <remarks>
        /// will check security internally, so assume the endpoint doesn't need to check security first
        /// </remarks>
        /// <returns></returns>
        IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
            string appPath, 
            string name, 
            AppQueryParameters more,
            string stream = null
        );
    }
}