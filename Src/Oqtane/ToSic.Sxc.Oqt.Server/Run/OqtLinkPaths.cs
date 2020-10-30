using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Run;


namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtLinkPaths: ILinkPaths
    {
        public OqtLinkPaths(IHttpContextAccessor contextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            _contextAccessor = contextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public HttpContext Current => _contextAccessor.HttpContext;

        #region Paths


        private string toWebAbsolute(string virtualPath)
        {
            virtualPath = virtualPath.TrimStart('~');
            //if (!virtualPath.StartsWith('/') && !virtualPath.StartsWith('\\'))
            //    virtualPath = "/" + virtualPath;
            return virtualPath.PrefixSlash().Forwardslash();
        }

        public string ToAbsolute(string virtualPath)
        {
            return toWebAbsolute(virtualPath);
        }
        public string ToAbsolute(string virtualPath, string subPath)
        {
            return toWebAbsolute(Path.Combine(virtualPath, subPath));
        }


        #endregion
    }
}
