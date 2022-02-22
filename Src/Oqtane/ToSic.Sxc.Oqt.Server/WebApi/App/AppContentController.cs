using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Data;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    /// <inheritdoc />
    /// <summary>
    /// Direct access to app-content items, simple manipulations etc.
    /// Should check for security at each standard call - to see if the current user may do this
    /// Then we can reduce security access level to anonymous, because each method will do the security check
    /// </summary>
    [ApiController]

    // Release routes
    [Route(WebApiConstants.AppRootNoLanguage+ "/{appPath}/content")]
    [Route(WebApiConstants.AppRootPathOrLang+ "/{appPath}/content")]
    [Route(WebApiConstants.AppRootPathNdLang + "/{appPath}/content")]
    [Route(WebApiConstants.AppRootNoLanguage + "/{appPath}/data")] // new, v13
    [Route(WebApiConstants.AppRootPathOrLang + "/{appPath}/data")] // new, v13
    [Route(WebApiConstants.AppRootPathNdLang + "/{appPath}/data")] // new, v13

    // Beta routes
    [Route( WebApiConstants.WebApiStateRoot + "/app/{appPath}/content/")]
    public class AppContentController: OqtStatefulControllerBase
    {
        private readonly Lazy<AppContent> _appContentLazy;

        #region DI / Constructor
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "AppCnt";

        public AppContentController(Lazy<AppContent> appContentLazy)
        {
            _appContentLazy = appContentLazy;
        }
        #endregion

        #region Get List / all of a certain content-type
        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet("{contentType}")]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IEnumerable<IDictionary<string, object>> GetEntities(string contentType, string appPath = null)
            => _appContentLazy.Value.Init(appPath, Log).GetItems(contentType, appPath);

        #endregion


        #region GetOne by ID / GUID

        [HttpGet("{contentType}/{id}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, object> GetOne(string contentType, string id, string appPath = null)
        {
            if(int.TryParse(id, out var intId))
                return GetAndSerializeOneAfterSecurityChecks(contentType,
                    entityApi => entityApi.GetOrThrow(contentType, intId), appPath);

            if (Guid.TryParse(id, out var guid))
                return GetAndSerializeOneAfterSecurityChecks(contentType,
                    entityApi => entityApi.GetOrThrow(contentType, guid), appPath);

#pragma warning disable S112 // General exceptions should never be thrown
            throw new Exception("id neither int/guid, can't process");
#pragma warning restore S112 // General exceptions should never be thrown
        }


        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method,
        /// ...then process/finish
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="getOne"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private IDictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath)
            => _appContentLazy.Value.Init(appPath, Log).GetOne(contentType, getOne, appPath);

        #endregion

        #region Create

        [HttpPost("{contentType}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public IDictionary<string, object> CreateOrUpdate([FromRoute] string contentType,
            [FromBody] Dictionary<string, object> newContentItem, [FromQuery] int? id = null,
            [FromQuery] string appPath = null)
            => _appContentLazy.Value.Init(appPath, Log)
                .CreateOrUpdate(contentType, newContentItem, id, appPath);

        #endregion

        #region Delete
        [HttpDelete("{contentType}/{id}")]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public void Delete(string contentType, string id, string appPath = null)
        {
            if (int.TryParse(id, out var intId))
            {
                _appContentLazy.Value.Init(appPath, Log).Delete(contentType, intId, appPath);
                return;
            }

            if (Guid.TryParse(id, out var guid))
            {
                _appContentLazy.Value.Init(appPath, Log).Delete(contentType, guid, appPath);
                return;
            }

#pragma warning disable S112 // General exceptions should never be thrown
            throw new Exception("id neither int/guid, can't process");
#pragma warning restore S112 // General exceptions should never be thrown
        }

        #endregion

    }
}
