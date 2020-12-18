using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.PublicApi
{
    public interface IAppQueryController
    {
        // will check security internally, so assume no requirements
        Dictionary<string, IEnumerable<Dictionary<string, object>>> Query(string name, bool includeGuid = false, string stream = null, int? appId = null);

        // will check security internally, so assume no requirements
        Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(string appPath, string name, string stream = null);
    }
}