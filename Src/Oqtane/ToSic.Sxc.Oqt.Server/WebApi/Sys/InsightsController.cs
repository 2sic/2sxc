using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Eav.Sys.Insights.InsightsControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + "/sys/[controller]/")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + "/sys/[controller]/")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + "/sys/[controller]/")]
    
[ApiController]
[OqtApiRequest("web-api.insights")]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class InsightsController : ControllerBase, IHasLog
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();


    private ContentResult Wrap(string contents) => base.Content(contents, "text/html");

    [HttpGet("{view}")]
    public ContentResult Details([FromRoute] string view, 
        [FromQuery] int? appId = null, [FromQuery] string key = null, [FromQuery] int? position = null,
        [FromQuery] string type = null, [FromQuery] bool? toggle = null, string nameId = null, string filter = default)
        => Wrap(Real.Details(view, appId, key, position, type, toggle, nameId, filter));

    #region Logging aspects

    /// <summary>
    /// Enable/disable logging of access to insights
    /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
    /// </summary>
    internal const bool InsightsLoggingEnabled = false;

    #endregion
}