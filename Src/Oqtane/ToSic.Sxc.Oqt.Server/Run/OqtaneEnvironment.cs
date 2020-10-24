using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneEnvironment : HasLog, IAppEnvironment
    {
        public OqtaneEnvironment(IHttp http, IZoneMapper zoneMapper, IUserResolver userResolver) : base("Mvc.Enviro")
        {
            _http = http;
            ZoneMapper = zoneMapper;
            _userResolver = userResolver;
        }
        private readonly IHttp _http;
        private readonly IUserResolver _userResolver;

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        public string DefaultLanguage => WipConstants.DefaultLanguage;

        public IZoneMapper ZoneMapper { get; }

        public IUser User => new OqtaneUser(_userResolver.GetUser());

        public IPagePublishing PagePublishing => Eav.Factory.Resolve<IPagePublishing>().Init(Log);


        //public string MapPath(string virtualPath) => _http.MapPath(virtualPath);



    }
}
