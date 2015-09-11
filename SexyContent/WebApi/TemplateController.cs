using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi
{
	[SupportedModules("2sxc,2sxc-app")]
	public class TemplateController : SxcApiController
	{

	    [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public IEnumerable<object> GetAll()
	    {
            var attributeSetList = Sexy.GetAvailableContentTypes(SexyContent.AttributeSetScope).ToList();
            var templateList = Sexy.Templates.GetAllTemplates();
            var templates = from c in templateList
                            join a in attributeSetList on c.ContentTypeStaticName equals a.StaticName into JoinedList
                            from a in JoinedList.DefaultIfEmpty()
                            select new
                            {
                                Id = c.TemplateId,
                                c.Name,
                                c.ContentTypeStaticName,
                                AttributeSetName = a != null ? a.Name : "No Content Type",
                                TemplatePath = c.Path,
                                DemoEntityID = c.ContentDemoEntity != null ? c.ContentDemoEntity.EntityId : new int?(),
                                c.IsHidden,
                                c.ViewNameInUrl,
                                c.Guid
                            };
	        return templates;
	    }

	    [HttpDelete]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public bool Delete(int templateId)
	    {
            Sexy.Templates.DeleteTemplate(templateId);
	        return true;
	    }
        

    }
}