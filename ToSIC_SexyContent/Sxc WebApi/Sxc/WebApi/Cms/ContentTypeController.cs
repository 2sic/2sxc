using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for Content-Type structures, fields etc.
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class ContentTypeController : SxcApiControllerBase, IContentTypeController
    {
        private Eav.WebApi.ContentTypeController _eavCtc;

	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("Api.SxcCTC");
            _eavCtc = new Eav.WebApi.ContentTypeController(Log);
	    }

	    #region Content-Type Get, Delete, Save
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> Get(int appId, string scope = null, bool withStatistics = false) 
            => _eavCtc.Get(appId, scope, withStatistics);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic Get(int appId, string contentTypeId, string scope = null) 
            => _eavCtc.GetSingle(appId, contentTypeId, scope);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public dynamic GetSingle(int appId, string contentTypeStaticName, string scope = null)
	    {
	        var permCheck = new MultiPermissionsTypes(CmsBlock, appId, contentTypeStaticName, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var exp))
                throw exp;

            if(!permCheck.UserCanWriteAndPublicFormsEnabled(out exp))
                throw exp;

            // if we got this far, permissions are ok
            return _eavCtc.GetSingle(appId, contentTypeStaticName, scope);
	    }

	    [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName) 
            => _eavCtc.Delete(appId, staticName);

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value as string);
            return _eavCtc.Save(appId, cleanList);
        }


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool CreateGhost(int appId, string sourceStaticName) 
            => _eavCtc.CreateGhost(appId, sourceStaticName);

	    #endregion


        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<ContentTypeFieldInfo> GetFields(int appId, string staticName)
        {
            Log.Add($"get fields for a:{appId} type:{staticName}");
	        var permCheck = new MultiPermissionsTypes(CmsBlock, appId, staticName, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var exp))
                throw exp;
            if(!permCheck.UserCanWriteAndPublicFormsEnabled(out exp))
                throw exp;

            return _eavCtc.GetFields(appId, staticName);
	    }

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string[] DataTypes(int appId) 
            => _eavCtc.DataTypes(appId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<InputTypeInfo> InputTypes(int appId) 
            => _eavCtc.InputTypes(appId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public int AddField(int appId, int contentTypeId, string staticName, string type, string inputType, int sortOrder) 
            => _eavCtc.AddField(appId, contentTypeId, staticName, type, inputType, sortOrder);

	    [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool DeleteField(int appId, int contentTypeId, int attributeId) 
            => _eavCtc.DeleteField(appId, contentTypeId, attributeId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Reorder(int appId, int contentTypeId, string newSortOrder) 
            => _eavCtc.Reorder(appId, contentTypeId, newSortOrder);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId) 
            => _eavCtc.SetTitle(appId, contentTypeId, attributeId);
        

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool UpdateInputType(int appId, int attributeId, string inputType) 
            => _eavCtc.UpdateInputType(appId, attributeId, inputType);

	    #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Rename(int appId, int contentTypeId, int attributeId, string newName)
            => _eavCtc.Rename(appId, contentTypeId, attributeId, newName);

	}
}