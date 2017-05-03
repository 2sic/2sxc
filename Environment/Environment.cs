using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment
{
    public class Environment: IEnvironment
    {
        public IPermissions Permissions { get; internal set; }

        public IZoneMapper ZoneMapper { get;  } = new Dnn7.ZoneMapper();

        public IUser User { get; } = new Dnn7.UserIdentity();
    }
}