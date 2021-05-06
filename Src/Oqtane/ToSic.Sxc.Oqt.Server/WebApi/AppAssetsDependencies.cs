using System;
using Microsoft.AspNetCore.Hosting;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class AppAssetsDependencies
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public Lazy<OqtAppFolder> OqtAppFolderLazy { get; }
        //public ILogManager Logger { get; }
        public SiteState SiteState { get; }


        public AppAssetsDependencies(
            IWebHostEnvironment hostingEnvironment, 
            Lazy<OqtAppFolder> oqtAppFolderLazy, 
            ILogManager logger, 
            SiteState siteState)
        {
            HostingEnvironment = hostingEnvironment;
            OqtAppFolderLazy = oqtAppFolderLazy;
            //Logger = logger;
            SiteState = siteState;
        }
    }
}
