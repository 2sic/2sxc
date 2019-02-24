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

        //[HttpGet]
        //public IEnumerable<Dictionary<string, object>> GetAssignedEntities(int assignmentObjectTypeId, string keyType, string key, string contentType, int appId) 
        //    => new Eav.WebApi.MetadataController(Log).Get(appId, assignmentObjectTypeId, keyType, key, contentType);

        [HttpGet]
        public IEnumerable<Dictionary<string, object>> Get(int appId, int targetType, string keyType, string key, string contentType)
            => new Eav.WebApi.MetadataController(Log).Get(appId, targetType, keyType, key, contentType);


    }
}