using System.IO;
using Microsoft.AspNetCore.Hosting;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneServerPaths : IServerPaths
    {
        public OqtaneServerPaths(IWebHostEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public string FullAppPath(string virtualPath) => Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);

        public string FullSystemPath(string virtualPath) => Path.Combine(_hostingEnvironment.WebRootPath, virtualPath);

        public string FullContentPath(string virtualPath) => Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);
    }

}
