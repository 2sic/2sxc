using System.Collections.Specialized;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys.VaryBy;
internal class CacheVaryByHelper
{
    internal static string VaryByToUrl(List<KeyValuePair<string, string>> all)
    {
        var nvc = all
            // Keep only relevant
            .Where(pair => pair.Value.HasValue())
            // Order so the same keys always result in the same string
            .OrderBy(pair => pair.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .Aggregate(new NameValueCollection(),
                (seed, pair) =>
                {
                    seed.Add(pair.Key, pair.Value);
                    return seed;
                });

        var asUrl = nvc.NvcToString();
        return asUrl;
    }

    internal static string VaryByParameters(IParameters parameters, string? names)
    {
        var all = parameters
            .Filter(names)
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ToList();
        return VaryByToUrl(all);
    }

}
