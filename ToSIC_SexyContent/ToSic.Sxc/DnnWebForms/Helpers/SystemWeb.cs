using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.Sxc.DnnWebForms.Helpers
{
    public static class SystemWeb
    {

        internal static IEnumerable<KeyValuePair<string, string>> GetUrlParams()
        {
#if NETSTANDARD
            throw new Exception("Not Yet Implemented in .net standard #TodoNetStandard");
#else
            var qs = HttpContext.Current?.Request.QueryString;
            if (qs == null) return null;
            // todo: try to get this into a helper system, to remove system.web dependncy from this class
            return (from string key in qs select new KeyValuePair<string, string>(key, qs[key]))
                .ToList();
#endif
        }
    }
}