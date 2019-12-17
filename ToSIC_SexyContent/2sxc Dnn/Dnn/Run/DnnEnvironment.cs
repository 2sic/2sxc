using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironment: HasLog, IAppEnvironment
    {
        public IZoneMapper ZoneMapper { get;  } = new DnnZoneMapper();

        public IUser User { get; } = new DnnUser();

        public IPagePublishing PagePublishing {get ; }


        public string MapAppPath(string virtualPath) => HostingEnvironment.MapPath(virtualPath);


        public  DnnEnvironment() : base("DNN.Enviro") { }

        public DnnEnvironment(ILog parentLog = null) : base("DNN.Enviro", parentLog, "()")
        {
            PagePublishing = Eav.Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
        }

        public string DefaultLanguage => PortalSettings.Current.DefaultLanguage;
    }
}