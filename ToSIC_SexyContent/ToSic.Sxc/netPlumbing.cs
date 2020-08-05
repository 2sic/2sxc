using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

#if NETSTANDARD
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
#else
using System.Web.Hosting;
using HttpContext = System.Web.HttpContext;
#endif

namespace ToSic.Sxc
{
    public static class netPlumbing
    {
        public static string VirtualPathUtility_Combine(string part1, string part2)
        {
#if NETSTANDARD
            throw new Exception("Not Yet Implemented in .net standard #TodoNetStandard");
#else
            return VirtualPathUtility.Combine(part1, part2);
#endif
        }

        public static string VirtualPathUtility_ToAbsolute(string virtualPath)
        {
#if NETSTANDARD
            throw new Exception("Not Yet Implemented in .net standard #TodoNetStandard");
#else
            return VirtualPathUtility.ToAbsolute(virtualPath);
#endif
        }

        public static string HostingEnvironment_MapPath(string virtualPath)
        {
#if NETSTANDARD
            throw new Exception("Not Yet Implemented in .net standard #TodoNetStandard");
#else
            return HostingEnvironment.MapPath(virtualPath);
#endif
        }

        public static HttpContext HttpContext_Current()
        {
#if NETSTANDARD
            throw new Exception("Not Yet Implemented in .net standard #TodoNetStandard");
#else
            return HttpContext.Current;
#endif
        }
    }
}
