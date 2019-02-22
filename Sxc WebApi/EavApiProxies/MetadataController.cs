using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.PublicApi;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <inheritdoc cref="IMetadataController" />
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class MetadataController : SxcApiControllerBase, IMetadataController
    {

        #region Content-Type Get, Delete, Save
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, string keyType, string key, string contentType, int appId) 
            => new Eav.WebApi.MetadataController(Log).GetAssignedEntities(assignmentObjectTypeId, keyType, key, contentType, appId);

	    #endregion


    }
}