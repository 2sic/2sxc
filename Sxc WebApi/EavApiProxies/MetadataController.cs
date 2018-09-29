using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class MetadataController : SxcApiControllerBase
	{

        #region Content-Type Get, Delete, Save
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, string keyType, string key, string contentType, int appId) 
            => new Eav.WebApi.MetadataController(Log).GetAssignedEntities(assignmentObjectTypeId, keyType, key, contentType, appId);

	    #endregion


    }
}