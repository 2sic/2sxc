using Microsoft.AspNetCore.Hosting;
using Oqtane.Shared;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public class AppAssetsDependencies
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public LazyInitLog<AppFolder> AppFolder { get; }
        public SiteState SiteState { get; }

        public AppAssetsDependencies(
            IWebHostEnvironment hostingEnvironment,
            LazyInitLog<AppFolder> appFolder, 
            SiteState siteState)
        {
            HostingEnvironment = hostingEnvironment;
            AppFolder = appFolder;
            SiteState = siteState;
        }
    }
}
