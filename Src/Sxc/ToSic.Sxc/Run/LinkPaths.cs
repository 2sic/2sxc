using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
#if NET451
using System.Web;
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Mvc;
using System.IO;
#endif

namespace ToSic.Sxc.Run
{
    public class LinkPaths: ILinkPaths
    {
        public static string AppFolderPlaceholder = "[AppFolder]";
        
#if NETSTANDARD
        public LinkPaths(IUrlHelper urlHelper) => _urlHelper = urlHelper;

        private readonly IUrlHelper _urlHelper;
#endif

        public string AsSeenFromTheDomainRoot(string virtualPath)
        {
#if NETSTANDARD
            return _urlHelper.Content(virtualPath);
#else
            return VirtualPathUtility.ToAbsolute(virtualPath);
#endif
        }
//        public string ToAbsolute(string virtualPath, string subPath)
//        {
//#if NETSTANDARD
//            return _urlHelper.Content(Path.Combine(virtualPath, subPath));
//#else
//            return VirtualPathUtility.Combine(virtualPath, subPath);
//#endif
//        }


        //public string AppAssetsBase(ISite site, IApp app) 
        //    //=> ToAbsolute(site.AppAssetsLinkTemplate.Replace(LinkPaths.AppFolderPlaceholder, app.Folder));
        //    => site.AppAssetsLinkTemplate.Replace(LinkPaths.AppFolderPlaceholder, app.Folder).ToAbsolutePathForwardSlash();
    }
}
