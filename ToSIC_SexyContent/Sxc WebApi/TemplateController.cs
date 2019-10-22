using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.AppEngine;
using ToSic.Eav.Apps;
using ToSic.Eav.Interfaces;

namespace ToSic.SexyContent.WebApi
{
	[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
	public class TemplateController : SxcApiControllerBase
	{

	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("2sSysC");
	    }


        [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public IEnumerable<object> GetAll(int appId)
        {
            Log.Add($"get all a#{appId}");
            var tm = TemplateManager(appId);

	        var attributeSetList = new AppRuntime(tm.ZoneId, tm.AppId, Log).ContentTypes.FromScope(Settings.AttributeSetScope).ToList();
            var templateList = tm.GetAllTemplates().ToList();
            Log.Add($"attrib list count:{attributeSetList.Count}, template count:{templateList.Count}");
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

	    private TemplateManager TemplateManager(int appId)
	    {
	        var zoneId = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId);
	        var tm = new TemplateManager(zoneId, appId, Log);
	        return tm;
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
	            Id = found?.ContentTypeId ?? 0,
	            Name = (found == null)? "no content type":  found.Name,
                DemoId = maybeEntity?.EntityId ?? 0,
                DemoTitle = maybeEntity?.GetBestValue("Title") ?? ""

            };
	    }

	    [HttpGet, HttpDelete]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public bool Delete(int appId, int id)
	    {
	        Log.Add($"delete a{appId}, t:{id}");
            var tm = TemplateManager(appId);
            tm.DeleteTemplate(id);
	        return true;
	    }
        

    }
}