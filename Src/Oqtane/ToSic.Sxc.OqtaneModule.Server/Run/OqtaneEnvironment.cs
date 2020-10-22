using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.OqtaneModule.Shared.Dev;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    public class OqtaneEnvironment : HasLog, IAppEnvironment
    {
        public OqtaneEnvironment(IHttp http, IZoneMapper zoneMapper) : base("Mvc.Enviro")
        {
            _http = http;
            ZoneMapper = zoneMapper;
        }
        private readonly IHttp _http;

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        public string DefaultLanguage => WipConstants.DefaultLanguage;

        public IZoneMapper ZoneMapper { get; }

        public IUser User { get; } = new OqtaneUser(WipConstants.NullUser);

        public IPagePublishing PagePublishing => Eav.Factory.Resolve<IPagePublishing>().Init(Log);


        public string MapPath(string virtualPath) => _http.MapPath(virtualPath);



    }
}
