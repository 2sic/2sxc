using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi.App;
using ToSic.Sxc.WebApi.App;


// TODO: #MissingFeature not yet implemented create/update/delete
// TODO: #MissingFeature not yet implemented GetContext using current context

namespace ToSic.Sxc.Mvc.WebApi.App
{
    [ApiController]
    [Route(WebApiConstants.WebApiRoot + "/app/{appPath}/content/")]
    public class AppDataController: SxcStatefulControllerBase, IAppDataController
    {
        #region DI / Constructor
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "AppCnt";
        
        #endregion

        #region Get List / all of a certain content-type
        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet("{contentType}")]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string appPath = null)
            => Eav.Factory.Resolve<AppContent>().Init(appPath, Log).GetItems(contentType, appPath);

        #endregion


        #region GetOne by ID / GUID

        [HttpGet("{contentType}/{id}")]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public Dictionary<string, object> GetOne(string contentType, string id, string appPath = null)
        {
            if(int.TryParse(id, out var intId))
                return GetAndSerializeOneAfterSecurityChecks(contentType,
                    entityApi => entityApi.GetOrThrow(contentType, intId), appPath);

            if (Guid.TryParse(id, out var guid))
                return GetAndSerializeOneAfterSecurityChecks(contentType,
                    entityApi => entityApi.GetOrThrow(contentType, guid), appPath);

            throw new Exception("id neither int/guid, can't process");
        }


        /// <summary>
        /// Preprocess security / context, then get the item based on an passed in method, 
        /// ...then process/finish
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="getOne"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetAndSerializeOneAfterSecurityChecks(string contentType, Func<IEnumerable<IEntity>, IEntity> getOne, string appPath) 
            => Eav.Factory.Resolve<AppContent>().Init(appPath, Log).GetOne(contentType, getOne, appPath);

        #endregion


    }
}
