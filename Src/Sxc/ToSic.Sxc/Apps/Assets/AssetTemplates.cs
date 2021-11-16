using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Apps.Assets
{
    public class TemplateKey
    {
        public const string CsHtml = "cshtml";
        public const string CsHtmlCode = "cshtml-code";
        public const string CsCode = "cs-code";
        public const string Api = "cs-api";
        public const string Token = "html-token";
        public const string CustomSearchCsCode = "cs-code-custom-search";
    }

    public class Extension
    {
        public const string Html = ".html";
        public const string Cshtml = ".cshtml";
        public const string CodeCshtml = ".code.cshtml";
        public const string Cs = ".cs";
        public const string ApiFolder = "api";
    }

    public class Purpose
    {
        public const string Auto = "auto";
        public const string Razor = "razor";
        public const string Token = "token";
        public const string Api = "api";
        public const string Search = "search";
    }

    [PrivateApi]
    public abstract class AssetTemplates: HasLog<IAssetTemplates>, IAssetTemplates
    {
        protected AssetTemplates() : base("SxcAss.Templt")
        {
        }

        public static TemplateInfo CsHtml = new TemplateInfo(TemplateKey.CsHtml, "CsHtml", Extension.Cshtml, Purpose.Razor);
        public static TemplateInfo CsHtmlCode = new TemplateInfo(TemplateKey.CsHtmlCode, "CsHtmlCode", Extension.CodeCshtml, Purpose.Razor);
        public static TemplateInfo CsCode = new TemplateInfo(TemplateKey.CsCode, "CsCode", Extension.Cs, Purpose.Auto);
        public static TemplateInfo Api = new TemplateInfo(TemplateKey.Api, "WebApi", Extension.Cs, Purpose.Api);
        public static TemplateInfo Token = new TemplateInfo(TemplateKey.Token, "Token", Extension.Html, Purpose.Token);
        public static TemplateInfo CustomSearchCsCode = new TemplateInfo(TemplateKey.CustomSearchCsCode, "CustomSearchCsCode", Extension.Cs, Purpose.Search);

        public virtual List<TemplateInfo> GetTemplates()
        {
            var callLog = Log.Call<List<TemplateInfo>>(nameof(GetTemplates));

            if (_templateInfos == null)
            {
                _templateInfos = new List<TemplateInfo>
                {
                    CsHtml,
                    CsHtmlCode,
                    CsCode,
                    Api,
                    Token,
                    CustomSearchCsCode
                };

                // prefill template body
                foreach (var templateInfo in _templateInfos)
                {
                    switch (templateInfo.Key)
                    {
                        case TemplateKey.CsHtml:
                            templateInfo.Body = DefaultCshtmlBody;
                            break;
                        case TemplateKey.CsHtmlCode:
                            templateInfo.Body = DefaultCodeCshtmlBody;
                            break;
                        case TemplateKey.CsCode:
                            templateInfo.Body = DefaultCsCode;
                            break;
                        case TemplateKey.Api:
                            templateInfo.Body = DefaultWebApiBody;
                            break;
                        case TemplateKey.Token:
                            templateInfo.Body = DefaultTokenHtmlBody;
                            break;
                        case TemplateKey.CustomSearchCsCode:
                            templateInfo.Body = CustomsSearchCsCode;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(templateInfo.Key), templateInfo.Key, null);
                    }
                }

            }

            return callLog(null, _templateInfos);
        }

        private static List<TemplateInfo> _templateInfos;

        public virtual TemplateInfo GetTemplateInfo(string key) => GetTemplates()
            .FirstOrDefault(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

        public virtual string GetTemplate(string key) => GetTemplateInfo(key).Body;

        internal string DefaultTokenHtmlBody =>
            @"<p>
    You successfully created your own template.
    Start editing it by hovering the ""Manage"" button and opening the ""Edit Template"" dialog.
</p>";

        internal string DefaultCshtmlBody { get; } = @"@inherits Custom.Hybrid.Razor12
@* This inherits statement gets you features like App, CmsContext, Data etc. - you can delete this comment *@

<div @Edit.TagToolbar(Content)>
    Put your content here
</div>";

        internal string DefaultCodeCshtmlBody { get; } = @"@inherits Custom.Hybrid.Razor12
@* This inherits statement gets you features like App, CmsContext, Data etc. - you can delete this comment *@
@using ToSic.Razor.Blade;

@functions {
  public string Hello() {
    return ""Hello from inner code"";
  }

  dynamic MessageHelper(string message) {
    return Tag.Div(message + ""!"");
  }
}
";

        public const string CsApiTemplateControllerName = "PleaseRenameController";

        // copied from the razor tutorial

        internal string DefaultWebApiBody =>
            @"#if NETCOREAPP // Oqtane
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else // DNN
using System.Web.Http;
using DotNetNuke.Web.Api;
#endif

[AllowAnonymous]      // define that all commands can be accessed without a login
// Inherit from Custom.Hybrid.Api12 to get features like App, CmsContext, Data etc.
// see https://docs.2sxc.org/web-api/custom/index.html
public class " + CsApiTemplateControllerName + @" : Custom.Hybrid.Api12
{
    [HttpGet]        // [HttpGet] says we're listening to GET requests
    public string Hello()
    {
        return ""Hello from the controller with ValidateAntiForgeryToken in /api"";
    }

    [HttpPost]        // [HttpPost] says we're listening to POST requests
    [ValidateAntiForgeryToken] // protects from the users not on your site (CSRF protection)
    public int Sum([FromBody] dynamic bodyJson) // post body { ""a"": 2, ""b"": 3 }
    {
        int a = bodyJson.a;
        int b = bodyJson.b;
        return a + b;
    }
}
";

        public const string CsCodeTemplateName = "PleaseRenameClass";

        internal string DefaultCsCode { get; } = @"// Important notes:
// - This class should have the same name as the file it's in
// - This inherits from Custom.Hybrid.Code12
//   which will automatically provide the common objects like App, CmsContext, Data etc.
//   from the current context to use in your code

public class " + CsCodeTemplateName + @" : Custom.Hybrid.Code12 {
  public string SayHello() {
    return ""Hello from shared functions!"";
  }
}
";

        internal abstract string CustomsSearchCsCode { get; }
    }
}
