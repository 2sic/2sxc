using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.WebApi.App;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.WebApi.App.AppDataControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.App;

/// <inheritdoc />
[ApiController]

// Release routes
[Route(OqtWebApiConstants.AppRootNoLanguage+ "/{appPath}/content")]
[Route(OqtWebApiConstants.AppRootPathOrLang+ "/{appPath}/content")]
[Route(OqtWebApiConstants.AppRootPathNdLang + "/{appPath}/content")]
[Route(OqtWebApiConstants.AppRootNoLanguage + "/{appPath}/data")] // new, v13
[Route(OqtWebApiConstants.AppRootPathOrLang + "/{appPath}/data")] // new, v13
[Route(OqtWebApiConstants.AppRootPathNdLang + "/{appPath}/data")] // new, v13

public class AppDataController: OqtStatefulControllerBase, IAppDataController
{
    public AppDataController(): base(RealController.LogSuffix) { }

    private RealController Real => GetService<RealController>();


    #region Get List / all of a certain content-type

    /// <inheritdoc />
    [HttpGet("{contentType}")]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = null)
        => Real.GetEntities(contentType, appPath);

    #endregion


    #region GetOne by ID / GUID

    /// <inheritdoc />
    [HttpGet("{contentType}/{id}")]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> GetOne(string contentType, string id, string appPath = null) 
        => Real.GetOne(contentType, id, appPath);

    #endregion

    #region Create

    /// <inheritdoc />
    [HttpPost("{contentType}")]
    [HttpPost("{contentType}/{id}")]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> CreateOrUpdate(
        string contentType,
        [FromBody] Dictionary<string, object> newContentItem,
        int? id = null,
        string appPath = null)
        =>Real.CreateOrUpdate(contentType, newContentItem, id, appPath);

    #endregion

    #region Delete

    /// <inheritdoc />
    [HttpDelete("{contentType}/{id}")]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public void Delete(string contentType, string id, string appPath = null) 
        => Real.Delete(contentType, id, appPath);

    #endregion

}