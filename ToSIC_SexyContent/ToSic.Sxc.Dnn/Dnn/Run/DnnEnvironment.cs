using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironment: HasLog, IAppEnvironment
    {
        #region Constructor and Init
        /// <summary>
        /// Constructor for DI, you must always call Init(...) afterwards
        /// </summary>
        public  DnnEnvironment() : base("DNN.Enviro") { }

        public IAppEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }
        #endregion

        public IZoneMapper ZoneMapper { get;  } = new DnnZoneMapper();

        public IUser User { get; } = new DnnUser();

        public IPagePublishing PagePublishing => _pagePublishing ?? (_pagePublishing = Eav.Factory.Resolve<IPagePublishing>().Init(Log));
        private IPagePublishing _pagePublishing;


        public string MapAppPath(string virtualPath) => HostingEnvironment.MapPath(virtualPath);

        public string DefaultLanguage => PortalSettings.Current.DefaultLanguage;

    }
}