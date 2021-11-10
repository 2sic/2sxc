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

        public static TemplateInfo CsHtml = new TemplateInfo("cshtml", "CsHtml", Extension.Cshtml, Purpose.Razor, "", Type.CsHtml);
        public static TemplateInfo CsHtmlCode = new TemplateInfo("cshtml-code", "CsHtmlCode", Extension.CodeCshtml, Purpose.Razor, "", Type.CsHtmlCode);
        public static TemplateInfo CsCode = new TemplateInfo("cs-code", "CsCode", Extension.Cs, Purpose.Auto, "", Type.CsCode);
        public static TemplateInfo Api = new TemplateInfo("cs-api", "WebApi", Extension.Cs, Purpose.Api, "", Type.WebApi);
        public static TemplateInfo Token = new TemplateInfo("html-token", "Token", Extension.Html, Purpose.Token, "", Type.Token);
        public static TemplateInfo CustomSearchCsCode = new TemplateInfo("cs-code-custom-search", "CustomSearchCsCode", Extension.Cs, Purpose.Search, "", Type.CustomSearchCsCode);

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
