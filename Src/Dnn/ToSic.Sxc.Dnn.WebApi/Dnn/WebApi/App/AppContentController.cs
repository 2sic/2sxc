using System;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Data;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [AllowAnonymous]
    public class AppContentController : SxcApiControllerBase
	{
        public AppContentController(): base("AppCnt") { }

        #region Get List / all of a certain content-type
        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = null) 
            => GetService<AppContent>().Init(appPath, Log).GetItems(contentType, appPath);

        #endregion

        #region GetOne by ID / GUID

	    [HttpGet]
	    [AllowAnonymous] // will check security internally, so assume no requirements
	    public IDictionary<string, object> GetOne(string contentType, int id, string appPath = null)
	        => GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, id), appPath);


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IDictionary<string, object> GetOne(string contentType, Guid guid, string appPath = null)
            => GetAndSerializeOneAfterSecurityChecks(contentType,
                entityApi => entityApi.GetOrThrow(contentType, guid), appPath);
        


        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method, 
        /// ...then process/finish
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="getOne"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private IDictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath) 
            => GetService<AppContent>().Init(appPath, Log).GetOne(contentType, getOne, appPath);

        #endregion

        #region Create

        [HttpPost]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, object> CreateOrUpdate([FromUri] string contentType,
            [FromBody] Dictionary<string, object> newContentItem, [FromUri] int? id = null,
            [FromUri] string appPath = null)
            => GetService<AppContent>().Init(appPath, Log)
                .CreateOrUpdate(contentType, newContentItem, id, appPath);

        #endregion

        #region Delete

        [HttpDelete]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, int id, [FromUri] string appPath = null) 
            => GetService<AppContent>().Init(appPath, Log).Delete(contentType, id, appPath);

        [HttpDelete]
	    [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, Guid guid, [FromUri] string appPath = null) 
            => GetService<AppContent>().Init(appPath, Log).Delete(contentType, guid, appPath);

        #endregion

    }
}
