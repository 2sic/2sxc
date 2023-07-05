using System;
using System.Linq;
using System.Text.RegularExpressions;
using WebApiConstants = ToSic.Sxc.Oqt.Server.WebApi.WebApiConstants;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    public static class OqtStartupHelper
    {

        public static bool IsSxcEndpoint(string path) => WebApiConstants.SxcEndpointPathRegexPatterns.Any(p => Regex.IsMatch(path, p, RegexOptions.IgnoreCase));

        public static bool IsSxcFallback(string path) => WebApiConstants.SxcFallbacks.Any(p => path.Contains(p.url, StringComparison.OrdinalIgnoreCase));
    }
}