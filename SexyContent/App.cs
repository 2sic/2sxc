using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class is currently only used to provide the App:Path placeholder in a template
    /// </summary>
    public class App
    {
        public int AppId { get; internal set; }
        public string Name { get; internal set; }
        public string Folder { get; internal set; }
        public bool Hidden { get; internal set; }
        public dynamic Configuration { get; internal set; }
        public dynamic Settings { get; internal set; }
        public dynamic Resources { get; internal set; }
        internal string StaticName { get; set; }

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
                return HttpContext.Current.Server.MapPath(appPath);
            }
        }
    }
}