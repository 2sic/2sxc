using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;

namespace ToSic.SexyContent.WebApi
{
	[SupportedModules("2sxc,2sxc-app")]
	public class TemplateController : SxcApiController
	{

	    [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public IEnumerable<object> GetAll(int appId)
	    {
            var sxc = Request.GetSxcOfModuleContext(appId);

            var attributeSetList = sxc.Templates.GetAvailableContentTypes(Settings.AttributeSetScope).ToList();
            var templateList = sxc.Templates.GetAllTemplates();
            var templates = from c in templateList
                            select new
                            {
                                Id = c.TemplateId,
                                c.Name,
                                ContentType = MiniCTSpecs(attributeSetList, c.ContentTypeStaticName, c.ContentDemoEntity),
                                PresentationType = MiniCTSpecs(attributeSetList, c.PresentationTypeStaticName, c.PresentationDemoEntity),
                                ListContentType = MiniCTSpecs(attributeSetList, c.ListContentTypeStaticName, c.ListContentDemoEntity),
                                ListPresentationType = MiniCTSpecs(attributeSetList, c.ListPresentationTypeStaticName, c.ListPresentationDemoEntity),
                                TemplatePath = c.Path,
                                c.IsHidden,
                                c.ViewNameInUrl,
                                c.Guid
                            };
	        return templates;
	    }

	    /// <summary>
	    /// Helper to prepare a quick-info about 1 content type
	    /// </summary>
	    /// <param name="allCTs"></param>
	    /// <param name="staticName"></param>
	    /// <param name="maybeEntity"></param>
	    /// <returns></returns>
	    private dynamic MiniCTSpecs(IEnumerable<IContentType> allCTs, string staticName, IEntity maybeEntity)
	    {
	        var found = allCTs.FirstOrDefault(ct => ct.StaticName == staticName);
	        return new
	        {
	            StaticName = staticName,
	            Id = found?.AttributeSetId ?? 0,
	            Name = (found == null)? "no content type":  found.Name,
                DemoId = maybeEntity?.EntityId ?? 0,
                DemoTitle = maybeEntity?.GetBestValue("Title") ?? ""

            };
	    }

	    [HttpGet, HttpDelete]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public bool Delete(int id)
	    {
            SxcContext.Templates.DeleteTemplate(id);
	        return true;
	    }
        

    }
}