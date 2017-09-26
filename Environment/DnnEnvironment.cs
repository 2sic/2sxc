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

        public  DnnEnvironment() : base("Dn.Enviro") { }

        public DnnEnvironment(Log parentLog = null) : base("DN.Enviro", parentLog)
        {
            PagePublishing = new PagePublishing(Log);
        }
    }
}