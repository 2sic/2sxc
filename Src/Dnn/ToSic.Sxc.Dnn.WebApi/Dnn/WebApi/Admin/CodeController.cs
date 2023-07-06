using System.Collections.Generic;
using System.Web.Http;
using ToSic.Lib.Logging;
using ToSic.Sxc.WebApi.Admin;
using RealController = ToSic.Sxc.WebApi.Admin.CodeControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    public class CodeController : DnnApiControllerWithFixes
    {
        public CodeController() : base(RealController.LogSuffix) { }
        private RealController Real => SysHlp.GetService<RealController>();

        [HttpGet]
        public IEnumerable<RealController.HelpItem> InlineHelp(string language)
        {
            Log.A($"InlineHelp:l:{language}");
            return Real.InlineHelp(language);
        }
    }
}
