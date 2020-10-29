using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtServerPaths : IServerPaths
    {
        public OqtServerPaths(IWebHostEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public string FullAppPath(string virtualPath) => FullContentPath(virtualPath);


        public string FullSystemPath(string virtualPath) => Path.Combine(_hostingEnvironment.WebRootPath, NoLeadingSlashes(virtualPath));

        public string FullContentPath(string virtualPath)
        {
            var path = NoLeadingSlashes(virtualPath);
            // sometimes the inbound path already contains the "Content" folder of oqtane, sometimes not
            if (path.StartsWith(OqtConstants.ContentSubfolder, StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Remove(0, OqtConstants.ContentSubfolder.Length);
                path = NoLeadingSlashes(path);
            }
            return Path.Combine(_hostingEnvironment.ContentRootPath, OqtConstants.ContentSubfolder, path);
        }

        private static string NoLeadingSlashes(string path) => path.TrimStart('/').TrimStart('\\');
    }

}
