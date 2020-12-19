using System.Collections.Generic;

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
        // will check security internally, so assume no requirements
        Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(string name, bool includeGuid = false, string stream = null, int? appId = null);

        // will check security internally, so assume no requirements
        Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(string appPath, string name, string stream = null);
    }
}