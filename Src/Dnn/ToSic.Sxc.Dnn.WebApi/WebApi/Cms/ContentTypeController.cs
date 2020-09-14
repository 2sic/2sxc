using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Security;

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

        private Eav.WebApi.ContentTypeApi EavCtc => _eavCtc ?? (_eavCtc = new Eav.WebApi.ContentTypeApi(Log));
        private Eav.WebApi.ContentTypeApi _eavCtc;

	    #region Content-Type Get, Delete, Save
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeDto> Get(int appId, string scope = null, bool withStatistics = false) 
            => EavCtc.Get(appId, scope, withStatistics);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IDictionary<string, string> Scopes(int appId)
        {
            return new AppRuntime(appId, false, Log).ContentTypes.ScopesWithLabels();
            //var scopes = appMan.Read.ContentTypes.GetScopes().ToList();

            ////// Make sure the "Default" scope is always included, otherwise it's missing on new apps
            ////if(!scopes.Contains(AppConstants.ScopeContentOld))
            ////    scopes.Add(AppConstants.ScopeContentOld);

            //var lookup = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            //{
            //    {AppConstants.ScopeContentOld, "Default"},
            //    {AppConstants.ScopeContentSystem, "System: CMS"},
            //    {AppConstants.ScopeApp, "System: App"},
            //    {Eav.Constants.ScopeSystem, "System: System"},
            //    {"System.DataSources", "System: DataSources"},
            //    {"System.Fields", "System: Fields"}
            //};

            //var dic = scopes
            //    .Select(s => new {value = s, name = lookup.TryGetValue(s, out var label) ? label : s})
            //    .OrderBy(s => s.name)
            //    .ToDictionary(s => s.value, s => s.name);
            //return dic;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) 
            => EavCtc.GetSingle(appId, contentTypeId, scope);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public ContentTypeDto GetSingle(int appId, string contentTypeStaticName, string scope = null)
	    {
	        var permCheck = new MultiPermissionsTypes().Init(GetContext(), GetApp(appId), contentTypeStaticName, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);

            if(!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw HttpException.PermissionDenied(error);

            // if we got this far, permissions are ok
            return EavCtc.GetSingle(appId, contentTypeStaticName, scope);
	    }

	    [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName) 
            => EavCtc.Delete(appId, staticName);

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return EavCtc.Save(appId, cleanList);
        }


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool CreateGhost(int appId, string sourceStaticName) 
            => EavCtc.CreateGhost(appId, sourceStaticName);

	    #endregion


        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<ContentTypeFieldDto> GetFields(int appId, string staticName)
        {
            Log.Add($"get fields for a:{appId} type:{staticName}");
	        var permCheck = new MultiPermissionsTypes().Init(GetContext(), GetApp(appId), staticName, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);
            if(!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw HttpException.PermissionDenied(error);

            return EavCtc.GetFields(appId, staticName);
	    }

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string[] DataTypes(int appId) 
            => EavCtc.DataTypes(appId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<InputTypeInfo> InputTypes(int appId) 
            => new AppRuntime(appId, true, Log).ContentTypes.GetInputTypes();

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public int AddField(int appId, int contentTypeId, string staticName, string type, string inputType, int sortOrder) 
            => EavCtc.AddField(appId, contentTypeId, staticName, type, inputType, sortOrder);

	    [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool DeleteField(int appId, int contentTypeId, int attributeId) 
            => EavCtc.DeleteField(appId, contentTypeId, attributeId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Reorder(int appId, int contentTypeId, string newSortOrder) 
            => EavCtc.Reorder(appId, contentTypeId, newSortOrder);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId) 
            => EavCtc.SetTitle(appId, contentTypeId, attributeId);
        

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool UpdateInputType(int appId, int attributeId, string inputType) 
            => EavCtc.UpdateInputType(appId, attributeId, inputType);

	    #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Rename(int appId, int contentTypeId, int attributeId, string newName)
            => EavCtc.Rename(appId, contentTypeId, attributeId, newName);

	}
}