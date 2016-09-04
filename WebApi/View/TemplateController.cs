using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.AppAssets;

namespace ToSic.SexyContent.WebApi.View
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class TemplateController : SxcApiController
    {

        //[HttpGet]
        //public AssetEditInfo Template(int templateId)
        //{
        //    var assetEditor = new AssetEditor(SxcContext, templateId, UserInfo, PortalSettings);
        //    assetEditor.EnsureUserMayEditAsset();
        //    return assetEditor.EditInfo();
        //}


        //[HttpPost]
        //public bool Template([FromUri] int templateId, AssetEditInfo template)
        //{
        //    var assetEditor = new AssetEditor(SxcContext, templateId, UserInfo, PortalSettings);
        //    assetEditor.EnsureUserMayEditAsset();
        //    assetEditor.Source = template.Code;
        //    return true;
        //}
    }

}