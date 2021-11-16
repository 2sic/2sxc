using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Apps.Assets
{
    [PrivateApi]
    public abstract class AssetTemplates: HasLog<IAssetTemplates>, IAssetTemplates
    {
        protected AssetTemplates() : base("SxcAss.Templt")
        {
        }

        public virtual string GetTemplate(string key)
        {
            var callLog = Log.Call<string>(key.ToString());
            string result;
            switch (key)
            {
                case Templates.Key.CsHtml:
                    result = DefaultCshtmlBody;
                    break;
                case Templates.Key.CsHtmlCode:
                    result = DefaultCodeCshtmlBody;
                    break;
                case Templates.Key.CsCode:
                    result = DefaultCsCode;
                    break;
                case Templates.Key.Api:
                    result = DefaultWebApiBody;
                    break;
                case Templates.Key.Token:
                    result = DefaultTokenHtmlBody;
                    break;
                case Templates.Key.CustomSearchCsCode:
                    result = CustomsSearchCsCode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }

            return callLog(null, result);
        }

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
