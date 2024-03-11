using ToSic.Eav.WebApi.App;
using static ToSic.Eav.WebApi.EavWebApiConstants;
using RealController = ToSic.Sxc.Backend.App.AppDataControllerReal;

namespace ToSic.Sxc.Dnn.Backend.App;

/// <inheritdoc />
[AllowAnonymous]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppDataController() : DnnSxcControllerBase(RealController.LogSuffix), IAppDataController
{
    private RealController Real => SysHlp.GetService<RealController>();

    #region Get List / all of a certain content-type

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = default, [FromUri(Name = ODataSelect)] string oDataSelect = default)
        => Real.GetEntities(contentType, appPath, oDataSelect: oDataSelect);

    #endregion

    #region GetOne by ID / GUID
        
    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> GetOne(string contentType, string guid, string appPath = default, [FromUri(Name = ODataSelect)] string oDataSelect = default) // this will handle Guid
        => Real.GetOne(contentType, guid, appPath, oDataSelect: oDataSelect);

    [HttpGet]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public IDictionary<string, object> GetOne(string contentType, int id, string appPath = default, [FromUri(Name = ODataSelect)] string oDataSelect = default) // this will handle int id
        => Real.GetOne(contentType, id.ToString(), appPath, oDataSelect: oDataSelect);

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