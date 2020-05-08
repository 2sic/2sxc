using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnAppFileSystemLoader : HasLog, IAppFileSystemLoader
    {
        public const string FieldFolderPrefix = "field-";
        public int AppId { get;  }

        public string Path { get; set; }

        protected readonly PortalSettings PortalSettings;

        public DnnAppFileSystemLoader(int appId, string path, PortalSettings portalSettings, ILog log): base("Dnn.AppStf", log)
        {
            var wrapLog = Log.Call($"{appId}, {path}, ...");
            AppId = appId;
            //Path = path;
            PortalSettings = portalSettings;

            var tenant = new DnnTenant(portalSettings);
            var fullPath = tenant.SxcPath + "/" + path + "/" + Settings.AppsSystemFolder;
            Path = HostingEnvironment.MapPath(fullPath);
            Log.Add("System path:" + Path);
            wrapLog(null);
        }

        public List<InputTypeInfo> FindInputTypes()
        {
            var wrapLog = Log.Call<List<InputTypeInfo>>();
            var di = new DirectoryInfo(Path);
            if (!di.Exists) return wrapLog("directory not found", new List<InputTypeInfo>());
            var inputFolders = di.GetDirectories(FieldFolderPrefix + "*");
            Log.Add($"found {inputFolders.Length} field-directories");

            var withIndexJs = inputFolders
                .Where(fld => fld.GetFiles("index.js").Any())
                .Select(fld => fld.Name).ToArray();
            Log.Add($"found {withIndexJs.Length} folders with index.js");

            var types = withIndexJs.Select(name =>
                {
                    var input = name.Substring(FieldFolderPrefix.Length);
                    return new InputTypeInfo(input, "Extension: " + name, "Field in App System", "", false,
                        $"[App:Path]/{Settings.AppsSystemFolder}/{name}/index.js", "", false);
                })
                .ToList();
            return wrapLog(null, types);
        }
    }
}
