using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.WebApi.App;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    /// <inheritdoc />
    [AllowAnonymous]
    public class AppDataController : SxcApiControllerBase<AppDataControllerReal>, IAppDataController
    {
        public AppDataController(): base(AppDataControllerReal.LogSuffix) { }

        #region Get List / all of a certain content-type
        
        /// <inheritdoc />
        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = null) 
            => Real.GetEntities(contentType, appPath);

        #endregion

        #region GetOne by ID / GUID
        
        /// <inheritdoc />
        [HttpGet]
	    [AllowAnonymous] // will check security internally, so assume no requirements
	    public IDictionary<string, object> GetOne(string contentType, string guid, string appPath = null) // this will handle Guid
            => Real.GetOne(contentType, guid, appPath);

        [HttpGet]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, object> GetOne(string contentType, int id, string appPath = null) // this will handle int id
            => Real.GetOne(contentType, id.ToString(), appPath);

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
}
