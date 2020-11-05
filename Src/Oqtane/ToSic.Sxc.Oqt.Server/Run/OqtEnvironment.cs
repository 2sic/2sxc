using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtEnvironment : HasLog, IAppEnvironment
    {
        public OqtEnvironment(IHttp http, IZoneMapper zoneMapper, IUserResolver userResolver, Lazy<IPagePublishing> pagePublishingLazy): base($"{OqtConstants.OqtLogPrefix}.Enviro")
        {
            _http = http;
            ZoneMapper = zoneMapper;
            _userResolver = userResolver;
            _pagePublishingLazy = pagePublishingLazy;
        }
        private readonly IHttp _http;
        private readonly IUserResolver _userResolver;
        private readonly Lazy<IPagePublishing> _pagePublishingLazy;

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        public string DefaultLanguage => WipConstants.DefaultLanguage;

        public IZoneMapper ZoneMapper { get; }

        public IUser User => new OqtUser(_userResolver.GetUser());

        public IPagePublishing PagePublishing => _pagePublishing ??= _pagePublishingLazy.Value.Init(Log);
        private IPagePublishing _pagePublishing;


        //public string MapPath(string virtualPath) => _http.MapPath(virtualPath);



    }
}
