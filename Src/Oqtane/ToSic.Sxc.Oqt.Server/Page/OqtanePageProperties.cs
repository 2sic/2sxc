using System.Text;
using Microsoft.AspNetCore.Html;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Page
{
    public class OqtanePageProperties: HasLog
    {
        #region Constructor and DI

        public OqtanePageProperties() : base("Mvc.PgProp")
        {
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
            // TODO #Oqtane
            if (AddContextMeta) headers.AppendLine(ContextHeader());

            if (AddJsCore) headers.AppendLine(Tag.Script().Src($"{OqtConstants.UiRoot}{InpageCms.CoreJs}").ToString());
            if (AddCmsJs) headers.AppendLine(Tag.Script().Src($"{OqtConstants.UiRoot}{InpageCms.EditJs}").ToString());
            if (AddCmsCss) headers.AppendLine(Tag.Link().Href($"{OqtConstants.UiRoot}{InpageCms.EditCss}").Rel("stylesheet").ToString());
            headers.AppendLine($"<meta />{Headers}<!-- end -->");
            return new HtmlString(headers.ToString());
        }

        public HtmlString FinalFooter()
        {
            return new HtmlString("<!-- footer from PageManager - should later contain the scripts etc.");
        }


        // TODO: #Oqtane
        public string ContextHeader()
        {
            var wrapLog = Log.Call<string>();

            var pageId = 0;
            var siteRoot = OqtConstants.SiteRoot;
            var apiRoot = siteRoot + WipConstants.WebApiPrefixFor1 + WebApiConstants.WebApiRoot + "/";
            var json = InpageCms.JsApiJson(pageId, siteRoot, apiRoot, AntiForgeryToken(), OqtConstants.UiRoot);

            var meta = Tag.Meta().Name(InpageCms.MetaName).Content(json).ToString();

            return wrapLog(meta, meta);
        }

        private string AntiForgeryToken()
        {
            return "anti forgery token todo";
        }

    }
}
