using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.TestStuff;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcEnvironment : HasLog, IAppEnvironment
    {
        public MvcEnvironment(IHttp http) : base("Mvc.Enviro")
        {
            _http = http;
        }
        private readonly IHttp _http;
        private IZoneMapper _zoneMapper;

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        public string DefaultLanguage => new MvcPortalSettings().DefaultLanguage;

        public IZoneMapper ZoneMapper => _zoneMapper ??= new MvcZoneMapper(_http);

        public IUser User { get; } = new MvcUser();

        public IPagePublishing PagePublishing => Eav.Factory.Resolve<IPagePublishing>().Init(Log);


        public string MapPath(string virtualPath) => _http.MapPath(virtualPath);



    }
}
