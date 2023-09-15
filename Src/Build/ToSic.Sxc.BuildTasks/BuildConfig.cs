using System.Collections.Generic;

namespace ToSic.Sxc.BuildTasks
{
    public class BuildConfig
    {
        public List<string> JsTargets { get; set; }
        public List<string> DnnTargets { get; set; }
        public List<string> OqtaneTargets { get; set; }
        public List<string> Sources { get; set; }
        public string DnnInstallPackage { get; set; }
        public string OqtaneInstallPackage { get; set; }
    }
}