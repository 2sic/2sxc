using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironment: HasLog, IAppEnvironment
    {
        #region Constructor and Init

        /// <summary>
        /// Constructor for DI, you must always call Init(...) afterwards
        /// </summary>
        public DnnEnvironment(IHttp http, ITenant tenant, IPagePublishing publishing) : base("DNN.Enviro")
        {
            _http = http;
            _tenant = tenant;
            PagePublishing = publishing.Init(Log);
        }

        private readonly IHttp _http;
        private readonly ITenant _tenant;

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }
        #endregion

        public IZoneMapper ZoneMapper => _zoneMapper ?? (_zoneMapper = new DnnZoneMapper().Init(Log));
        private IZoneMapper _zoneMapper;

        public IUser User { get; } = new DnnUser();

        public IPagePublishing PagePublishing { get; }

        public string MapPath(string virtualPath) => _http.MapPath(virtualPath);

        public string DefaultLanguage => _tenant.DefaultLanguage;

    }
}