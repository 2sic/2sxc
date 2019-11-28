using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Data;
using ToSic.SexyContent;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Apps;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.Cms
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
            var cms = new CmsRuntime(appId, Log, true);

            var attributeSetList = cms.ContentTypes.FromScope(Settings.AttributeSetScope).ToList();
            var templateList = cms.Views.GetAll().ToList();
            Log.Add($"attrib list count:{attributeSetList.Count}, template count:{templateList.Count}");
            var templates = from c in templateList
                            select new
                            {
                                Id = c.Id,
                                c.Name,
                                ContentType = MiniCTSpecs(attributeSetList, c.ContentType, c.ContentItem),
                                PresentationType = MiniCTSpecs(attributeSetList, c.PresentationType, c.PresentationItem),
                                ListContentType = MiniCTSpecs(attributeSetList, c.HeaderType, c.HeaderItem),
                                ListPresentationType = MiniCTSpecs(attributeSetList, c.HeaderPresentationType, c.HeaderPresentationItem),
                                TemplatePath = c.Path,
                                c.IsHidden,
                                ViewNameInUrl = c.UrlIdentifier,
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
            // todo: must add extra security to only allow zone change if host user
	        Log.Add($"delete a{appId}, t:{id}");
            var app = Factory.App(appId, false);


            var cms = new CmsManager(app, Log);
            cms.Views.DeleteTemplate(id);
	        return true;
	    }
        

    }
}