using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
	/// <summary>
	/// Web API Controller for the Pipeline Designer UI
	/// </summary>
	public class ContentTypeController : SxcApiController
	{
        private readonly Eav.WebApi.ContentTypeController _eavController;
        public ContentTypeController()
        {
            _eavController = new Eav.WebApi.ContentTypeController();
        }

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetFields(string staticName)
        {
			return _eavController.GetFields(this.App.AppId, staticName);
        }

	}
}