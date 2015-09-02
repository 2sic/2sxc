using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.EAV.FormlyEditUI
{
	/// <summary>
	/// Web API Controller for the Pipeline Designer UI
	/// </summary>
	public class ContentTypeController : SxcApiController
	{
        private readonly Eav.ManagementUI.FormlyEditUI.ContentTypeController _eavController;
        public ContentTypeController()
        {
            _eavController = new Eav.ManagementUI.FormlyEditUI.ContentTypeController();
        }

        /// <summary>
        /// Returns the configuration for a content type
        /// </summary>
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetContentTypeConfiguration(string contentTypeName)
        {
            return _eavController.GetContentTypeConfiguration(this.App.ZoneId, this.App.AppId, contentTypeName);
        }

	}
}