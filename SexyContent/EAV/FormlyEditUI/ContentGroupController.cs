using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.ViewAPI
{
	[SupportedModules("2sxc,2sxc-app")]
	public class ContentGroupController : SxcApiController
	{

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		public object Get([FromUri] Guid contentGroupGuid)
		{
			var contentGroup = Sexy.ContentGroups.GetContentGroup(contentGroupGuid);

			if (contentGroup == null)
				throw new Exception("ContentGroup with Guid " + contentGroupGuid + " does not exist.");

			return new
			{
				Guid = contentGroup.ContentGroupGuid,
				Content = contentGroup.Content.Select(e => e == null ? new int?() : e.EntityId).ToArray(),
				Presentation = contentGroup.Presentation.Select(e => e == null ? new int?() : e.EntityId).ToArray(),
				ListContent = contentGroup.ListContent.Select(e => e.EntityId).ToArray(),
				ListPresentation = contentGroup.ListPresentation.Select(e => e.EntityId).ToArray(),
				Template = contentGroup.Template == null ? null : new {
					contentGroup.Template.Name,
					contentGroup.Template.ContentTypeStaticName,
					contentGroup.Template.PresentationTypeStaticName,
					contentGroup.Template.ListContentTypeStaticName,
					contentGroup.Template.ListPresentationTypeStaticName
				}
			};
		}

	}
}