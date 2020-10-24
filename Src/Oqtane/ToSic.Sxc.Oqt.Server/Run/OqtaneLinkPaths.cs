using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Run;


namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneLinkPaths: ILinkPaths
    {
        public OqtaneLinkPaths(IHttpContextAccessor contextAccessor, IWebHostEnvironment hostingEnvironment
            //IUrlHelperFactory urlHelperFactory, 
            //IActionContextAccessor actionContextAccessor
            )
        {
            //Current = contextAccessor.HttpContext;
            _contextAccessor = contextAccessor;
            _hostingEnvironment = hostingEnvironment;
            //_urlHelperFactory = urlHelperFactory;
            //_actionContextAccessor = actionContextAccessor;
        }

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        //private readonly IUrlHelperFactory _urlHelperFactory;
        //private readonly IActionContextAccessor _actionContextAccessor;
        //private IUrlHelper UrlHelper => _urlHlp ??= _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext );
        private IUrlHelper _urlHlp;
        public HttpContext Current => _contextAccessor.HttpContext;

        #region Paths


        private string toWebAbsolute(string virtualPath)
        {
            virtualPath = virtualPath.TrimStart('~');
            if (!virtualPath.StartsWith('/') && !virtualPath.StartsWith('\\'))
                virtualPath = "/" + virtualPath;
            return virtualPath;
        }

        public string ToAbsolute(string virtualPath)
        {
            return /*UrlHelper.Content*/toWebAbsolute(virtualPath);
        }
        public string ToAbsolute(string virtualPath, string subPath)
        {
            return /*UrlHelper.Content*/toWebAbsolute(Path.Combine(virtualPath, subPath));
        }


        #endregion
    }
}
