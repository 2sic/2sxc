using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Security;
using ContentTypeApi = ToSic.Eav.WebApi.ContentTypeApi;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// Web API Controller for Content-Type structures, fields etc.
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class ContentTypeController : SxcApiControllerBase, IContentTypeController
    {
        protected override string HistoryLogName => "Api.SxcCTC";

	    #region Content-Type Get, Delete, Save
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeDto> Get(int appId, string scope = null, bool withStatistics = false) 
            => new ContentTypeApi(Log).Get(appId, scope, withStatistics);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IDictionary<string, string> Scopes(int appId) 
            => new AppRuntime(appId, false, Log).ContentTypes.ScopesWithLabels();

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) 
            => new ContentTypeApi(Log).GetSingle(appId, contentTypeId, scope);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public ContentTypeDto GetSingle(int appId, string contentTypeStaticName, string scope = null)
        {
            SecurityHelpers.ThrowIfNotEditorOrIsPublicForm(GetContext(), GetApp(appId), contentTypeStaticName, Log);
            return new ContentTypeApi(Log).GetSingle(appId, contentTypeStaticName, scope);
	    }

        [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName) 
            => new ContentTypeApi(Log).Delete(appId, staticName);

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return new ContentTypeApi(Log).Save(appId, cleanList);
        }


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool CreateGhost(int appId, string sourceStaticName) 
            => new ContentTypeApi(Log).CreateGhost(appId, sourceStaticName);

	    #endregion


        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<ContentTypeFieldDto> GetFields(int appId, string staticName)
        {
            SecurityHelpers.ThrowIfNotEditorOrIsPublicForm(GetContext(), GetApp(appId), staticName, Log);
            return new ContentTypeApi(Log).GetFields(appId, staticName);
	    }

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string[] DataTypes(int appId) 
            => new ContentTypeApi(Log).DataTypes(appId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<InputTypeInfo> InputTypes(int appId) 
            => new AppRuntime(appId, true, Log).ContentTypes.GetInputTypes();

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public int AddField(int appId, int contentTypeId, string staticName, string type, string inputType, int sortOrder) 
            => new ContentTypeApi(Log).AddField(appId, contentTypeId, staticName, type, inputType, sortOrder);

	    [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool DeleteField(int appId, int contentTypeId, int attributeId) 
            => new ContentTypeApi(Log).DeleteField(appId, contentTypeId, attributeId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Reorder(int appId, int contentTypeId, string newSortOrder) 
            => new ContentTypeApi(Log).Reorder(appId, contentTypeId, newSortOrder);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId) 
            => new ContentTypeApi(Log).SetTitle(appId, contentTypeId, attributeId);
        

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool UpdateInputType(int appId, int attributeId, string inputType) 
            => new ContentTypeApi(Log).UpdateInputType(appId, attributeId, inputType);

	    #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Rename(int appId, int contentTypeId, int attributeId, string newName)
            => new ContentTypeApi(Log).Rename(appId, contentTypeId, attributeId, newName);

	}
}