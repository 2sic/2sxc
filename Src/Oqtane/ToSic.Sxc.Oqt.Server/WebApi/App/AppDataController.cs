using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.WebApi.App;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    /// <inheritdoc />
    [ApiController]

    // Release routes
    [Route(WebApiConstants.AppRootNoLanguage+ "/{appPath}/content")]
    [Route(WebApiConstants.AppRootPathOrLang+ "/{appPath}/content")]
    [Route(WebApiConstants.AppRootPathNdLang + "/{appPath}/content")]
    [Route(WebApiConstants.AppRootNoLanguage + "/{appPath}/data")] // new, v13
    [Route(WebApiConstants.AppRootPathOrLang + "/{appPath}/data")] // new, v13
    [Route(WebApiConstants.AppRootPathNdLang + "/{appPath}/data")] // new, v13

    public class AppDataController: OqtStatefulControllerBase<AppDataControllerReal>, IAppDataController
    {
        public AppDataController(): base(AppDataControllerReal.LogSuffix) { }

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
}
