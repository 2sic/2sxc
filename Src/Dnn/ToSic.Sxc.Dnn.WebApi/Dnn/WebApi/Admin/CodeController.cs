using System.Collections.Generic;
using System.Web.Http;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    public class CodeController : SxcApiControllerBase<CodeControllerReal>
    {
        public CodeController() : base(CodeControllerReal.LogSuffix) { }

        [HttpGet]
        public IEnumerable<CodeControllerReal.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);
    }
}
