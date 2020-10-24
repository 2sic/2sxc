using System.IO;
using Microsoft.AspNetCore.Hosting;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneServerPaths : IServerPaths
    {
        public OqtaneServerPaths(IWebHostEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public string FullAppPath(string virtualPath) => FullContentPath(virtualPath);


        public string FullSystemPath(string virtualPath) => Path.Combine(_hostingEnvironment.WebRootPath, NoLeadingSlashes(virtualPath));

        public string FullContentPath(string virtualPath) => Path.Combine(_hostingEnvironment.ContentRootPath, OqtConstants.ContentSubfolder, NoLeadingSlashes(virtualPath));
        private static string NoLeadingSlashes(string path) => path.TrimStart('/').TrimStart('\\');
    }

}
