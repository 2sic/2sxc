using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironment: HasLog, IAppEnvironment
    {
        #region Constructor and Init

        /// <summary>
        /// Constructor for DI, you must always call Init(...) afterwards
        /// </summary>
        public DnnEnvironment(IHttp http, IServerPaths serverPaths, ITenant tenant, IPagePublishing publishing, IZoneMapper zoneMapper) : base("DNN.Enviro")
        {
            _http = http;
            _serverPaths = serverPaths;
            _tenant = tenant;
            PagePublishing = publishing.Init(Log);
            ZoneMapper = zoneMapper.Init(Log);
        }

        private readonly IHttp _http;
        private readonly IServerPaths _serverPaths;
        private readonly ITenant _tenant;

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            if (_tenant.Id == Eav.Constants.NullId)
                Log.Add("Warning - tenant isn't ready - will probably cause errors");
            return this;
        }
        #endregion

        public IZoneMapper ZoneMapper { get; }

        public IUser User { get; } = new DnnUser();

        public IPagePublishing PagePublishing { get; }

        //public string MapPath(string virtualPath) => _http.MapPath(virtualPath);

        public string DefaultLanguage => _tenant.DefaultLanguage;

    }
}