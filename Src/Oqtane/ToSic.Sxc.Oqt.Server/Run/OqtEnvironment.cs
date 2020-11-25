using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtEnvironment : HasLog, IEnvironment
    {
        public OqtEnvironment(IUserResolver userResolver): base($"{OqtConstants.OqtLogPrefix}.Enviro")
        {
            _userResolver = userResolver;
        }
        private readonly IUserResolver _userResolver;

        public IEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        public IUser User => new OqtUser(_userResolver.GetUser());

    }
}
