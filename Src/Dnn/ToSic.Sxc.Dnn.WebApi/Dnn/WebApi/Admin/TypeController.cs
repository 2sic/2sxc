using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ContentTypeApi = ToSic.Eav.WebApi.ContentTypeApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Web API Controller for Content-Type structures, fields etc.
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class TypeController : SxcApiControllerBase, ITypeController
    {
        protected override string HistoryLogName => "Api.Types";

        private ContentTypeApi Backend => new ContentTypeApi(Log);

        [HttpGet]
        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false) 
            => Backend.Get(appId, scope, withStatistics);

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        public IDictionary<string, string> Scopes(int appId) 
            => new AppRuntime(appId, false, Log).ContentTypes.ScopesWithLabels();

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => Backend.GetSingle(appId, contentTypeId, scope);


        [HttpDelete]
        public bool Delete(int appId, string staticName) => Backend.Delete(appId, staticName);

	    [HttpPost]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return Backend.Save(appId, cleanList);
        }

        /// <summary>
        /// Used to be GET ContentType/CreateGhost
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="sourceStaticName"></param>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool AddGhost(int appId, string sourceStaticName) => Backend.CreateGhost(appId, sourceStaticName);



	    [HttpPost]
        public void SetTitle(int appId, int contentTypeId, int attributeId) 
            => Backend.SetTitle(appId, contentTypeId, attributeId);


	}
}