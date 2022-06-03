using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.WebApi.Admin;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    public class CodeController : SxcApiControllerBase<CodeControllerReal>
    {
        public CodeController() : base(CodeControllerReal.LogSuffix) { }

        [HttpGet]
        public IEnumerable<CodeControllerReal.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);
    }
}
