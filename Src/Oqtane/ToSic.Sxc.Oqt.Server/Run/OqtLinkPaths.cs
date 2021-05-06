using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
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
            return virtualPath.PrefixSlash().ForwardSlash();
        }

        public string AsSeenFromTheDomainRoot(string virtualPath)
        {
            return toWebAbsolute(virtualPath);
        }
        //public string ToAbsolute(string virtualPath, string subPath)
        //{
        //    return toWebAbsolute(Path.Combine(virtualPath, subPath));
        //}

        //public string AppAssetsBase(ISite site, IApp app) 
        //    => toWebAbsolute(site.AppAssetsLinkTemplate.Replace(LinkPaths.AppFolderPlaceholder, app.Folder));

        #endregion
    }
}
