using System.Collections.Generic;
using System.Web.Http;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Sxc.Backend.Admin.CodeControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeController() : SxcApiControllerBase(RealController.LogSuffix)
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    public IEnumerable<RealController.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);
}