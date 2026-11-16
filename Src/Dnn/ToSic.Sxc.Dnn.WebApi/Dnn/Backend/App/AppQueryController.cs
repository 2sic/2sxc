using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.App.Sys;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Dnn.WebApi.Sys;

namespace ToSic.Sxc.Dnn.Backend.App;

[AllowAnonymous] // All functions will check security internally, so assume no requirements
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppQueryController() : DnnSxcControllerBase(AppWebApiQueryRunSvc.LogSuffix), IAppQueryController
{
    private AppWebApiQueryRunSvc Real => SysHlp.GetService<AppWebApiQueryRunSvc>();
    private AppQueryContextResolver CtxResolver => SysHlp.GetService<AppQueryContextResolver>();

    // GET is separated from POST to solve HttpResponseException that happens when
    // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

    [HttpGet]
    public IDictionary<string, IEnumerable<EavLightEntity>> Query(
        [FromUri] string name,
        [FromUri] int? appId = null,
        [FromUri] string stream = null,
        [FromUri] bool? includeGuid = false
    ) => Real.RunQuery(CtxResolver.GetCtxAndLookupForAppIdOrContext(appId),
        queryName: name, more: null, stream: stream, includeGuid: includeGuid);

    [HttpPost]
    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost
    ([FromUri] string name,
        [FromBody] QueryParametersDtoFromClient more,
        [FromUri] int? appId = null,
        [FromUri] string stream = null,
        [FromUri] bool? includeGuid = false
    ) => Real.RunQuery(CtxResolver.GetCtxAndLookupForAppIdOrContext(appId),
        queryName: name, more: more, stream: stream, includeGuid: includeGuid);

    [HttpGet] 
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
        [FromUri] string appPath,
        [FromUri] string name,
        [FromUri] string stream = null
    ) => Real.RunQuery(CtxResolver.GetCtxAndLookupForAppPath(appPath),
        queryName: name, more: null, stream: stream);

    [HttpPost]
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(
        [FromUri] string appPath,
        [FromUri] string name,
        [FromBody] QueryParametersDtoFromClient more,
        [FromUri] string stream = null
    ) => Real.RunQuery(CtxResolver.GetCtxAndLookupForAppPath(appPath),
        queryName: name, more: more, stream: stream);
}