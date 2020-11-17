using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Data;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;


// TODO: #MissingFeature not yet implemented create/update/delete
// TODO: #MissingFeature not yet implemented GetContext using current context

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [ApiController]
    [Route( WebApiConstants.WebApiStateRoot + "/app/{appPath}/content/")]
    public class AppContentController: SxcStatefulControllerBase
    {
        private readonly Lazy<AppContent> _appContentLazy;

        #region DI / Constructor
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "AppCnt";

        public AppContentController(StatefulControllerDependencies dependencies, Lazy<AppContent> appContentLazy) : base(dependencies)
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
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, string appPath = null)
            => _appContentLazy.Value.Init(GetContext(), NoBlock, Log).GetItems(contentType, appPath);

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
            => _appContentLazy.Value.Init(GetContext(), NoBlock, Log).GetOne(contentType, getOne, appPath);

        #endregion

    }
}
