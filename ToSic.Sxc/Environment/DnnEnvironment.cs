using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.Environment
{
    public class DnnEnvironment: HasLog, IEnvironment<PortalSettings>
    {
        public IPermissions Permissions { get; internal set; }

        public IZoneMapper<PortalSettings> ZoneMapper { get;  } = new ZoneMapper();
        IZoneMapper IEnvironment.ZoneMapper => ZoneMapper;

        public IUser User { get; } = new UserIdentity();

        public IPagePublishing PagePublishing {get ; }


        public string MapPath(string virtualPath) => HostingEnvironment.MapPath(virtualPath);


        public  DnnEnvironment() : base("DNN.Enviro") { }

        public DnnEnvironment(Log parentLog = null) : base("DNN.Enviro", parentLog)
        {
            PagePublishing = new PagePublishing(Log);
        }
    }
}