using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class is currently only used to provide the App:Path placeholder in a template
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
        
        public string AppGuid { get; set; }

        public App(int appId, int zoneId, IDataSource parentSource = null)
        {
            AppId = appId;
            ZoneId = zoneId;
            //InitialSource = parentSource;
        }

        public string Path
        {
            get
            {
                var appPath = System.IO.Path.Combine(SexyContent.AppBasePath(), Folder);
                return VirtualPathUtility.ToAbsolute(appPath);
            }
        }
        public string PhysicalPath
        {
            get
            {
                var appPath = System.IO.Path.Combine(SexyContent.AppBasePath(), Folder);
                return HostingEnvironment.MapPath(appPath);
            }
        }

        private IDataSource _data;
        public IDataSource Data
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