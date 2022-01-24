using Microsoft.AspNetCore.Hosting;
using Oqtane.Shared;
using System;
using ToSic.Sxc.Oqt.Server.Apps;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class AppAssetsDependencies
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public Lazy<OqtAppFolder> OqtAppFolderLazy { get; }
        public SiteState SiteState { get; }

        public AppAssetsDependencies(
            IWebHostEnvironment hostingEnvironment, 
            Lazy<OqtAppFolder> oqtAppFolderLazy, 
            SiteState siteState)
        {
            HostingEnvironment = hostingEnvironment;
            OqtAppFolderLazy = oqtAppFolderLazy;
            SiteState = siteState;
        }
    }
}
