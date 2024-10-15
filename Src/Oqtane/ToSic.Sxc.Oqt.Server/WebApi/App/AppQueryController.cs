using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Admin.App;
using ToSic.Eav.WebApi.Admin.Query;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.App.AppQueryControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.App;

// Release routes
[Route(OqtWebApiConstants.AppRootNoLanguage)]
[Route(OqtWebApiConstants.AppRootPathOrLang)]
[Route(OqtWebApiConstants.AppRootPathAndLang)]

[AllowAnonymous] // All functions will check security internally, so assume no requirements
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppQueryController() : OqtStatefulControllerBase(RealController.LogSuffix), IAppQueryController
{
    private RealController Real => GetService<RealController>();

    // GET is separated from POST to solve HttpResponseException that happens when
    // 'content-type' header is missing (or in GET request) on the endpoint that has [FromBody] in signature

    [HttpGet("{appPath}/" + AppParts.Query + "/{name}")]
    [HttpGet("{appPath}/" + AppParts.Query + "/{name}/{stream}")]
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQuery(
        [FromRoute] string appPath,
        [FromRoute] string name,
        [FromRoute] string stream = null
    ) => Real.PublicQuery(appPath, name, stream);

    [HttpPost("{appPath}/" + AppParts.Query + "/{name}")]
    [HttpPost("{appPath}/" + AppParts.Query + "/{name}/{stream}")]
    public IDictionary<string, IEnumerable<EavLightEntity>> PublicQueryPost(
        [FromRoute] string appPath,
        [FromRoute] string name,
        QueryParameters more,
        [FromRoute] string stream = null
    ) => Real.PublicQueryPost(appPath, name, more, stream);

    [HttpGet($"{AppParts.Auto}/{AppParts.Query}" + "/{name}")]
    [HttpGet($"{AppParts.Auto}/{AppParts.Query}" + "/{name}/{stream?}")]
    public IDictionary<string, IEnumerable<EavLightEntity>> Query(
        [FromRoute] string name,
        [FromQuery] int? appId = null,
        [FromRoute] string stream = null,
        [FromQuery] bool includeGuid = false
    ) => Real.Query(name, appId, stream, includeGuid);

    [HttpPost($"{AppParts.Auto}/{AppParts.Query}" + "/{name}")]
    [HttpPost($"{AppParts.Auto}/{AppParts.Query}" + "/{name}/{stream?}")]
    public IDictionary<string, IEnumerable<EavLightEntity>> QueryPost(
        [FromRoute] string name,
        QueryParameters more,
        [FromQuery] int? appId = null,
        [FromRoute] string stream = null,
        [FromQuery] bool includeGuid = false
    ) => Real.QueryPost(name, more, appId, stream, includeGuid);
}