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
    public class ContentTypeController : SxcApiController
	{
        private readonly Eav.WebApi.ContentTypeController eavCtc;
        public ContentTypeController()
        {
            eavCtc = new Eav.WebApi.ContentTypeController();
            //eavCtc.SetUser(Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);

        }

        #region Content-Type Get, Delete, Save
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> Get(int appId, string scope = null, bool withStatistics = false)
        {
            return eavCtc.Get(appId, scope, withStatistics);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic Get(int appId, string contentTypeId, string scope = null)
        {
            return eavCtc.GetSingle(appId, contentTypeId, scope);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic GetSingle(int appId, string contentTypeStaticName, string scope = null)
        {
            return eavCtc.GetSingle(appId, contentTypeStaticName, scope);
        }

        [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName)
        {
            return eavCtc.Delete(appId, staticName);
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Save(int appId, Dictionary<string, string> item)
        {
            return eavCtc.Save(appId, item);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool CreateGhost(int appId, string sourceStaticName)
        {
            return eavCtc.CreateGhost(appId, sourceStaticName);
        }

        #endregion


        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetFields(int appId, string staticName)
        {
			return eavCtc.GetFields(appId, staticName);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string[] DataTypes(int appId)
        {
            return eavCtc.DataTypes(appId);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> InputTypes(int appId)
        {
            return eavCtc.InputTypes(appId);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public int AddField(int appId, int contentTypeId, string staticName, string type, string inputType, int sortOrder)
        {
            return eavCtc.AddField(appId, contentTypeId, staticName, type, inputType, sortOrder);
        }

        [HttpGet]
        [HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool DeleteField(int appId, int contentTypeId, int attributeId)
        {
            return eavCtc.DeleteField(appId, contentTypeId, attributeId);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Reorder(int appId, int contentTypeId, string newSortOrder)
        {
            return eavCtc.Reorder(appId, contentTypeId, newSortOrder);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId)
        {
            eavCtc.SetTitle(appId, contentTypeId, attributeId);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool UpdateInputType(int appId, int attributeId, string inputType)
        {
            return eavCtc.UpdateInputType(appId, attributeId, inputType);
        }
        #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void Rename(int appId, int contentTypeId, int attributeId, string newName)
        {
            eavCtc.Rename(appId, contentTypeId, attributeId, newName);
        }

    }
}