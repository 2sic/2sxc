using System.Text;
using Microsoft.AspNetCore.Html;
using ToSic.Lib.Logging;
using ToSic.Sxc.Edit;
using ToSic.Razor.Blade;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Mvc.Web
{
    public class MvcPageProperties: HasLog
    {
        private readonly JsApiCache _jsApiCache;

        #region Constructor and DI

        public MvcPageProperties(JsApiCache jsApiCache) : base("Mvc.PgProp")
        {
            _jsApiCache = jsApiCache;
        }

        #endregion

        public bool AddContextMeta = true;

        public bool AddJsCore = true;
        //public bool AddJsAdvanced = true;
        public bool AddCmsJs = true;
        public bool AddCmsCss = true;

        public string Headers = "";


        public HtmlString FinalHeaders()
        {
            var headers = new StringBuilder();
            headers.AppendLine("<!-- 2sxc headers -->");
            if (AddContextMeta) headers.AppendLine(ContextHeader());
            if(AddJsCore) headers.AppendLine(Tag.Script().Src($"{MvcConstants.UiRoot}{InpageCms.CoreJs}").ToString());
            if(AddCmsJs) headers.AppendLine(Tag.Script().Src($"{MvcConstants.UiRoot}{InpageCms.EditJs}").ToString());
            if(AddCmsCss) headers.AppendLine(Tag.Link().Href($"{MvcConstants.UiRoot}{InpageCms.EditCss}").Rel("stylesheet").ToString());
            headers.AppendLine($"<meta />{Headers}<!-- end -->");
            return new HtmlString(headers.ToString());
        }

        public HtmlString FinalFooter()
        {
            return new HtmlString("<!-- footer from PageManager - should later contain the scripts etc.");
        }


        public string ContextHeader()
        {
            var wrapLog = Log.Fn<string>();

            var pageId = 0;
            var siteRoot = MvcConstants.SiteRoot;
            var apiRoot = siteRoot + WebApi.WebApiConstants.WebApiRoot + "/";
            var jsApiJson = _jsApiCache.JsApiJson(
                platform: "MVC", 
                pageId: pageId, 
                siteRoot: () => siteRoot, 
                apiRoot: () => apiRoot, 
                appApiRoot: () => apiRoot,
                uiRoot: () => "TODO",
                rvtHeader: "TODO",
                rvt: AntiForgeryToken,
                dialogQuery: MvcConstants.UiRoot);
            var json = InpageCms.JsApiJson(jsApiJson);

            var meta = Tag.Meta().Name(InpageCms.MetaName).Content(json).ToString();

            return wrapLog.ReturnAndLog(meta);
        }

        private string AntiForgeryToken()
        {
            return "anti forgery token todo";
        }

    }
}
