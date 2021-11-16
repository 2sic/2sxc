using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Apps.Assets
{
    public class TemplateKey
    {
        public const string CsHtml = "cshtml-hybrid";
        public const string CsHtmlCode = "cshtml-code-hybrid";
        public const string CsCode = "cs-code-hybrid";
        public const string Api = "cs-api-hybrid";
        public const string Token = "html-token";
        public const string CustomSearchCsCode = "cs-code-custom-search-dnn";
    }

    public class Extension
    {
        public const string Html = ".html";
        public const string Cshtml = ".cshtml";
        public const string CodeCshtml = ".code.cshtml";
        public const string Cs = ".cs";
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
    public abstract class AssetTemplates : HasLog<IAssetTemplates>, IAssetTemplates
    {
        protected AssetTemplates() : base("SxcAss.Templt")
        {
        }

        private List<TemplateInfo> TemplateInfos()
        {
            return new List<TemplateInfo>
            {
                new TemplateInfo(TemplateKey.CsHtml, "cshtml hybrid", Extension.Cshtml, Purpose.Razor, DefaultCshtmlBody, "razor page hybrid template"),
                new TemplateInfo(TemplateKey.CsHtmlCode, "cshtml code hybrid", Extension.CodeCshtml, Purpose.Razor, DefaultCodeCshtmlBody, "razor page c# code hybrid template"),
                new TemplateInfo(TemplateKey.CsCode, "c# code hybrid", Extension.Cs, Purpose.Auto, DefaultCsCode, "c# code hybrid template"),
                new TemplateInfo(TemplateKey.Api, "WebApi hybrid", Extension.Cs, Purpose.Api, DefaultWebApiBody, "c# WebApi controller hybrid template"),
                new TemplateInfo(TemplateKey.Token, "html token", Extension.Html, Purpose.Token, DefaultTokenHtmlBody, "html token template"),
                new TemplateInfo(TemplateKey.CustomSearchCsCode, "dnn custom search c# code", Extension.Cs, Purpose.Search, CustomsSearchCsCode, "custom search c# code to customize how dnn search treats data of view, see https://r.2sxc.org/customize-search"),
                new TemplateInfo("readme", "README.md", ".md", "assets", Readme, ".md file"),
                new TemplateInfo("txt", "text file", ".txt", "assets", Txt, "simple textual file"),
            };
        }

        public List<TemplateInfo> GetTemplates()
            {
            var callLog = Log.Call<List<TemplateInfo>>(nameof(GetTemplates));

            if (_templateInfos == null) _templateInfos = TemplateInfos();

            return callLog(null, _templateInfos);
        }

        private static List<TemplateInfo> _templateInfos;

        public string GetTemplate(string key) => GetTemplateInfo(key).Body;

        private TemplateInfo GetTemplateInfo(string key) => GetTemplates()
            .FirstOrDefault(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

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

        internal string Readme = @"# Readme

A standard README file.
";

        internal string Txt = @"Simple text file.
";
    }
}
