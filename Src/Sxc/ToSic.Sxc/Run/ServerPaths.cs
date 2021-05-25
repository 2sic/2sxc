#if NETSTANDARD
using System.IO;
using Microsoft.AspNetCore.Hosting;
#else
using System.Web.Hosting;
#endif
using ToSic.Eav.Run;


namespace ToSic.Sxc.Run
{
    /// <summary>
    /// In the default implementation, all things have the same root so content-path and app-path
    /// are calculated the same way.
    /// </summary>
    public class ServerPaths: IServerPaths
    {
#if NETSTANDARD
        public ServerPaths(IHostingEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;

        private readonly IHostingEnvironment _hostingEnvironment;
        
        protected string MapContentPath(string virtualPath)
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);
        }
#else
        protected string MapContentPath(string virtualPath) => HostingEnvironment.MapPath(virtualPath);
#endif


        /// <inheritdoc />
        public string FullAppPath(string virtualPath) => MapContentPath(virtualPath);

        ///// <inheritdoc />
        //public string FullSystemPath(string virtualPath) => MapContentPath(virtualPath);

        /// <inheritdoc />
        public string FullContentPath(string virtualPath) => MapContentPath(virtualPath);
    }
}
