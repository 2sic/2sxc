using System.Web.Hosting;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.Environment
{
    public class DnnEnvironment: HasLog, IEnvironment
    {
        public IPermissions Permissions { get; internal set; }

        public IZoneMapper ZoneMapper { get;  } = new ZoneMapper();

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