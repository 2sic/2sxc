using System.Linq;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class gives access to the App-object - for the data and things like the App:Path placeholder in a template
    /// </summary>
    public class App
    {
        public int AppId { get; internal set; }
        public int ZoneId { get; internal set; }
        public string Name { get; internal set; }
        public string Folder { get; internal set; }
        public bool Hidden { get; internal set; }
        public dynamic Configuration { get; internal set; }
        public dynamic Settings { get; internal set; }
        public dynamic Resources { get; internal set; }

        //private IDataSource InitialSource { get; set; }
        internal PortalSettings OwnerPS { get; set; }
        
        public string AppGuid { get; set; }

        public App(int appId, int zoneId, PortalSettings ownerPS, IDataSource parentSource = null)
        {
            AppId = appId;
            ZoneId = zoneId;
            //InitialSource = parentSource;
            OwnerPS = ownerPS;
        }

        /// <summary>
        /// needed to initialize data - must always happen a bit later because the show-draft info isn't available when creating the first App-object.
        /// todo: later this should be moved to initialization of this object
        /// </summary>
        /// <param name="showDrafts"></param>
        internal void InitData(bool showDrafts)
        {
            // ToDo: Remove this as soon as App.Data getter on App class is fixed #1 and #2
            if (_data == null)
            {
                // ModulePermissionController does not work when indexing, return false for search
                var initialSource = SexyContent.GetInitialDataSource(ZoneId, AppId, showDrafts);

                _data = DataSource.GetDataSource<DataSources.App>(initialSource.ZoneId,
                    initialSource.AppId, initialSource, initialSource.ConfigurationProvider);
                var defaultLanguage = "";
                var languagesActive = SexyContent.GetCulturesWithActiveState(OwnerPS.PortalId, ZoneId).Any(c => c.Active);
                if (languagesActive)
                    defaultLanguage = OwnerPS.DefaultLanguage;
                Data.DefaultLanguage = defaultLanguage;
                Data.CurrentUserName = OwnerPS.UserInfo.Username;
            }
        }

        public string Path
        {
            get
            {
                var appPath = System.IO.Path.Combine(SexyContent.AppBasePath(OwnerPS), Folder);
                return VirtualPathUtility.ToAbsolute(appPath);
            }
        }
        public string PhysicalPath
        {
            get
            {
                var appPath = System.IO.Path.Combine(SexyContent.AppBasePath(OwnerPS), Folder);
                return HostingEnvironment.MapPath(appPath);
            }
        }

        //private IDataSource _data;
        //public IDataSource Data
        private DataSources.App _data;
        public DataSources.App Data
        {
            get
            {
                //if (_data == null)
                //{
                //    // ToDo: #1 Care about "showDrafts" (instead of setting it to false)
                //    // ToDo: #2 this property gets temporarily overwritten in WebPageBase, remove this if #1 is fixed
                //    var initialSource = SexyContent.GetInitialDataSource(ZoneId, AppId, false);
                //    _data = DataSource.GetDataSource<ToSic.Eav.DataSources.App>(ZoneId, AppId, initialSource, initialSource.ConfigurationProvider);
                //}
                return _data;
            }
            set { _data = value; }
        }

    }
}