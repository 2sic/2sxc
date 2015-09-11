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
        public IEnumerable<dynamic> Get(string scope = null, bool withStatistics = false)
        {
            return eavCtc.Get(App.AppId, scope, withStatistics);
        }

        [HttpGet]
        public IContentType Get(string contentTypeId, string scope = null)
        {
            return eavCtc.Get(App.AppId, contentTypeId, scope);
        }

        [HttpDelete]
        public bool Delete(string staticName)
        {
            return eavCtc.Delete(App.AppId, staticName);
        }

        [HttpPost]
        public bool Save(Dictionary<string, string> item)
        {
            return eavCtc.Save(App.AppId, item);
        }
        #endregion


        #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetFields(string staticName)
        {
			return eavCtc.GetFields(this.App.AppId, staticName);
        }

        #endregion


    }
}