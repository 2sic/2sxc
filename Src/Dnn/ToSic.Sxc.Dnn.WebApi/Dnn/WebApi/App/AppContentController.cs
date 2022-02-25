using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.WebApi.App;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    /// <inheritdoc />
    [AllowAnonymous]
    public class AppContentController : SxcApiControllerBase<AppContentControllerReal>, IAppContentController
    {
        public AppContentController(): base(AppContentControllerReal.LogSuffix) { }

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
	    public IDictionary<string, object> GetOne(string contentType, string id, string appPath = null)
	        => Real.GetOne(contentType, id, appPath);

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
        public void Delete(string contentType, string id, [FromUri] string appPath = null) 
            => Real.Delete(contentType, id, appPath);

        #endregion

    }
}
