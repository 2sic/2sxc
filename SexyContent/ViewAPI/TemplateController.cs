using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.SexyContent.ViewManager;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class TemplateController : SxcApiController
    {

        [HttpGet]
        public TemplateInfo Template(int templateId)
        {
            var viewEditor = new TemplateEditor(Sexy, templateId, UserInfo, PortalSettings);

            viewEditor.EnsureUserMayEditTemplate();
            var templ = viewEditor.Template;

            var t = new TemplateInfo();
            t.Type = templ.Type;
            t.FileName = templ.Path;
            t.Code = viewEditor.Code;
            t.Name = templ.Name;
            t.HasList = templ.UseForList;
            t.HasApp = App.Name != "Content";
            t.AppId = App.AppId;
            t.TypeContent = templ.ContentTypeStaticName;
            t.TypeContentPresentation = templ.PresentationTypeStaticName;
            t.TypeList = templ.ListContentTypeStaticName;
            t.TypeListPresentation = templ.ListPresentationTypeStaticName;

            return t;
        }


        [HttpPost]
        public bool Template([FromUri] int templateId, TemplateInfo template)
        {
            var viewEditor = new TemplateEditor(Sexy, templateId, UserInfo, PortalSettings);

            viewEditor.EnsureUserMayEditTemplate();

            viewEditor.Code = template.Code;

            return true;
        }
    }

    public class TemplateInfo
    {
        public string 
            Name,
            Code,
            FileName,
            TypeContent,
            TypeContentPresentation,
            TypeList,
            TypeListPresentation;
        public string Type = "Token";
        public bool HasList;
        public bool HasApp;
        public int AppId;
        public Dictionary<string,string> Streams = new Dictionary<string, string>();
    }
}