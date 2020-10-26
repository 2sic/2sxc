using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Oqt.Shared.Run;

namespace ToSic.Sxc.Oqt.Server.Page
{
    public class OqtAssetsAndHeaders: HasLog, IOqtAssetsAndHeader
    {
        #region Constructor and DI

        public OqtAssetsAndHeaders() : base("Mvc.PgProp")
        {
        }

        #endregion

        public bool AddContextMeta => true;

        public bool AddJsCore = true;
        //public bool AddJsAdvanced = true;
        public bool AddCmsJs = true;
        public bool AddCmsCss = true;

        public string Headers = "";


        public IEnumerable<string> Scripts()
        {
            var list = new List<string>();
            if (AddJsCore) list.Add($"{OqtConstants.UiRoot}{InpageCms.CoreJs}");
            if (AddCmsJs) list.Add($"{OqtConstants.UiRoot}{InpageCms.EditJs}");
            return list;
        }

        public IEnumerable<string> Styles()
        {
            if (!AddCmsCss) return Array.Empty<string>();
            var list = new List<string>  { $"{OqtConstants.UiRoot}{InpageCms.EditCss}" };
            return list;
        }

        //public HtmlString FinalFooter()
        //{
        //    return new HtmlString("<!-- footer from PageManager - should later contain the scripts etc.");
        //}


        // TODO: #Oqtane
        public string ContextMetaHeader()
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
