#if NET451
using System.Web.Hosting;
#else
using System.IO;
using Microsoft.AspNetCore.Hosting;
#endif
using ToSic.Eav.Run;


namespace ToSic.Sxc.Run
{
    public class ServerPaths: IServerPaths
    {
#if NETSTANDARD
        public ServerPaths(IHostingEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;

        private readonly IHostingEnvironment _hostingEnvironment;
#endif

        protected string MapContentPath(string virtualPath)
        {
#if NETSTANDARD
            return Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);
#else
            return HostingEnvironment.MapPath(virtualPath);
#endif
        }

        /// <inheritdoc />
        public string FullAppPath(string virtualPath) => MapContentPath(virtualPath);

        /// <inheritdoc />
        public string FullSystemPath(string virtualPath) => MapContentPath(virtualPath);

        /// <inheritdoc />
        public string FullContentPath(string virtualPath) => MapContentPath(virtualPath);
    }
}
