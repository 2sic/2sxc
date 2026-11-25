using System.Web.Http.Controllers;
using ToSic.Eav.WebApi.App.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.App.AppDataControllerReal;

namespace ToSic.Sxc.Dnn.Backend.App;

/// <inheritdoc />
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[AllowAnonymous]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppDataController : DnnApiController, IAppDataController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    /// <summary>
    /// The app data controller probably needs this for every request.
    /// </summary>
    /// <param name="controllerContext"></param>
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        base.Initialize(controllerContext);
        SysHlp.PublishContextBlockOnly(Request, Log);
    }

    #endregion


    private RealController Real => SysHlp.GetServiceNoExCtx<RealController>();

    #region Get List / all of a certain content-type

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = default, [FromUri] IDictionary<string, string> queryParams = null)
        => Real.GetEntities(contentType, appPath, uri: Request.RequestUri);

    #endregion

    #region GetOne by ID / GUID
        
    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> GetOne(string contentType, string guid, string appPath = default, [FromUri] IDictionary<string, string> queryParams = null) // this will handle Guid
        => Real.GetOne(contentType, guid, appPath, uri: Request.RequestUri);

    [HttpGet]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> GetOne(string contentType, int id, string appPath = default, [FromUri] IDictionary<string, string> queryParams = null) // this will handle int id
        => Real.GetOne(contentType, id.ToString(), appPath, uri: Request.RequestUri);

    #endregion

    #region Create

    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> CreateOrUpdate(
        [FromUri] string contentType,
        [FromBody] Dictionary<string, object> newContentItem, 
        [FromUri] int? id = null,
        [FromUri] string appPath = null)
        => Real.CreateOrUpdate(contentType, newContentItem, id, appPath);

    #endregion

    #region Delete

    /// <inheritdoc />
    [HttpDelete]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public void Delete(string contentType, string guid, [FromUri] string appPath = null) // this will handle Guid
        => Real.Delete(contentType, guid, appPath);

 
    [HttpDelete]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public void Delete(string contentType, int id, [FromUri] string appPath = null) // this will handle int id
        => Real.Delete(contentType, id.ToString(), appPath);

    #endregion

}