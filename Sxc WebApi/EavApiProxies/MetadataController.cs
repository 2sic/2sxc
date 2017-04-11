using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class MetadataController : SxcApiController
	{
        private readonly Eav.WebApi.MetadataController eavMeta;
        public MetadataController()
        {
            eavMeta = new Eav.WebApi.MetadataController();
            eavMeta.SetUser(Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);

            // eavMeta.GetAssignedEntities();
        }

        #region Content-Type Get, Delete, Save
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, string keyType, string key, string contentType, int? appId = null)
        {
            return eavMeta.GetAssignedEntities(assignmentObjectTypeId, keyType, key, contentType, appId);
        }
        
        #endregion


    }
}