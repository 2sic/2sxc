using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.Environment
{
    public class DnnEnvironment: HasLog, IAppEnvironment
    {
        public IZoneMapper ZoneMapper { get;  } = new ZoneMapper();

        public IUser User { get; } = new DnnUser();

        public IPagePublishing PagePublishing {get ; }


        public string MapAppPath(string virtualPath) => HostingEnvironment.MapPath(virtualPath);


        public  DnnEnvironment() : base("DNN.Enviro") { }

        public DnnEnvironment(ILog parentLog = null) : base("DNN.Enviro", parentLog) 
            => PagePublishing =  Eav.Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);

        public string DefaultLanguage => PortalSettings.Current.DefaultLanguage;
    }
}