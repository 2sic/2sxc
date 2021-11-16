using System.Collections.Generic;

namespace ToSic.Sxc.Apps.Assets
{
    public class Templates
    {
        private readonly IAssetTemplates _assetTemplates;

        public Templates(IAssetTemplates assetTemplates)
        {
            _assetTemplates = assetTemplates;
        }

        public static class Key
        {
            public const string CsHtml = "cshtml";
            public const string CsHtmlCode = "cshtml-code";
            public const string CsCode = "cs-code";
            public const string Api = "cs-api";
            public const string Token = "html-token";
            public const string CustomSearchCsCode = "cs-code-custom-search";
        }

        public static TemplateInfo CsHtml = new TemplateInfo(Key.CsHtml, "CsHtml", Extension.Cshtml, Purpose.Razor);
        public static TemplateInfo CsHtmlCode = new TemplateInfo(Key.CsHtmlCode, "CsHtmlCode", Extension.CodeCshtml, Purpose.Razor);
        public static TemplateInfo CsCode = new TemplateInfo(Key.CsCode, "CsCode", Extension.Cs, Purpose.Auto);
        public static TemplateInfo Api = new TemplateInfo(Key.Api, "WebApi", Extension.Cs, Purpose.Api);
        public static TemplateInfo Token = new TemplateInfo(Key.Token, "Token", Extension.Html, Purpose.Token);
        public static TemplateInfo CustomSearchCsCode = new TemplateInfo(Key.CustomSearchCsCode, "CustomSearchCsCode", Extension.Cs, Purpose.Search);

        public static List<TemplateInfo> GetTemplates()
        {
            if (_getTemplates == null)
            {
                _getTemplates = new List<TemplateInfo>
                {
                    CsHtml,
                    CsHtmlCode,
                    CsCode,
                    Api,
                    Token,
                    CustomSearchCsCode
                };
            }
            return _getTemplates;
        }

        private static List<TemplateInfo> _getTemplates;
    }
}
