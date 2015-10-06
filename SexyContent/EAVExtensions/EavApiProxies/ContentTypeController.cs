using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Entities.Content;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ContentTypeController : SxcApiController
	{
        private readonly Eav.WebApi.ContentTypeController eavCtc;
        public ContentTypeController()
        {
            eavCtc = new Eav.WebApi.ContentTypeController();
        }

        #region Content-Type Get, Delete, Save
        [HttpGet]
        public IEnumerable<dynamic> Get(int appId, string scope = null, bool withStatistics = false)
        {
            return eavCtc.Get(appId, scope, withStatistics);
        }

        [HttpGet]
        public IContentType Get(int appId, string contentTypeId, string scope = null)
        {
            return eavCtc.GetSingle(appId, contentTypeId, scope);
        }

        [HttpGet]
        public IContentType GetSingle(int appId, string contentTypeStaticName, string scope = null)
        {
            return eavCtc.GetSingle(appId, contentTypeStaticName, scope);
        }

        [HttpDelete]
        public bool Delete(int appId, string staticName)
        {
            return eavCtc.Delete(appId, staticName);
        }

        [HttpPost]
        public bool Save(int appId, Dictionary<string, string> item)
        {
            return eavCtc.Save(appId, item);
        }
        #endregion


        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetFields(int appId, string staticName)
        {
			return eavCtc.GetFields(appId, staticName);
        }

        [HttpGet]
        public string[] DataTypes()
        {
            return eavCtc.DataTypes(App.AppId);
        }

        [HttpGet]
        public int AddField(int appId, int contentTypeId, string staticName, string type, int sortOrder)
        {
            return eavCtc.AddField(appId, contentTypeId, staticName, type, sortOrder);
        }

        [HttpDelete]
        public bool DeleteField(int appId, int contentTypeId, int attributeId)
        {
            return eavCtc.DeleteField(appId, contentTypeId, attributeId);
        }

        [HttpGet]
        public void SetTitle(int appId, int contentTypeId, int attributeId)
        {
            eavCtc.SetTitle(appId, attributeId, contentTypeId);
        }

        #endregion


    }
}