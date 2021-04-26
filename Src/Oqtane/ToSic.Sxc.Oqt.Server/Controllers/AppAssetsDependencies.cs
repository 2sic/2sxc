using Microsoft.AspNetCore.Hosting;
using Oqtane.Infrastructure;
using System;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public class AppAssetsDependencies
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public Lazy<OqtAppFolder> OqtAppFolderLazy { get; }
        public ILogManager Logger { get; }


        public AppAssetsDependencies(IWebHostEnvironment hostingEnvironment, Lazy<OqtAppFolder> oqtAppFolderLazy, ILogManager logger)
        {
            HostingEnvironment = hostingEnvironment;
            OqtAppFolderLazy = oqtAppFolderLazy;
            Logger = logger;
        }
    }
}
