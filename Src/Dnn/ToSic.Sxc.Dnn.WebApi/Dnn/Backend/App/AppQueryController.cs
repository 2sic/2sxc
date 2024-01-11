using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Admin.App;
using ToSic.Eav.WebApi.Admin.Query;
using RealController = ToSic.Sxc.Backend.App.AppQueryControllerReal;

namespace ToSic.Sxc.Dnn.Backend.App;

[AllowAnonymous] // All functions will check security internally, so assume no requirements
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppQueryController() : DnnSxcControllerBase(RealController.LogSuffix), IAppQueryController
{
    private RealController Real => SysHlp.GetService<RealController>();

    // GET is separated from POST to solve HttpResponseException that happens when
    // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

    [HttpGet]
    public IDictionary<string, IEnumerable<EavLightEntity>> Query([FromUri] string name,
        [FromUri] int? appId = null,
        [FromUri] string stream = null,
        [FromUri] bool includeGuid = false
    ) => Real.Query(name, appId, stream, includeGuid);

    [HttpPost]
    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost([FromUri] string name,
        [FromBody] QueryParameters more,
        [FromUri] int? appId = null,
        [FromUri] string stream = null,
        [FromUri] bool includeGuid = false
    ) => Real.QueryPost(name, more, appId, stream, includeGuid);

    [HttpGet] 
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
        [FromUri] string appPath,
        [FromUri] string name,
        [FromUri] string stream = null
    ) => Real.PublicQuery(appPath, name, stream);

    [HttpPost]
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(
        [FromUri] string appPath,
        [FromUri] string name,
        [FromBody] QueryParameters more,
        [FromUri] string stream = null
    ) => Real.PublicQueryPost(appPath, name, more, stream);
}