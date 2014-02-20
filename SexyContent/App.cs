using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent
{
    /// <summary>
    /// The app class is currently only used to provide the App:Path placeholder in a template
    /// </summary>
    public class App
    {
        public App()
        {
            Name = "Content";
        }

        public int AppId { get; internal set; }
        public string Name { get; internal set; }
        public string Folder { get; internal set; }
        public bool Hidden { get; internal set; }
        public dynamic Configuration { get; internal set; }
        public dynamic Settings { get; internal set; }
        public dynamic Resources { get; internal set; }

        public string Path
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(SexyContent.GetTemplatePathRoot(this.Location))
            }
        }
        public string PhysicalPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(SexyContent.GetTemplatePathRoot(this.Location))
            }
        }
    }
}