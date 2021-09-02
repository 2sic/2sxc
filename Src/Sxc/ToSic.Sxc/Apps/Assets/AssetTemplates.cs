using System;
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

        public virtual string GetTemplate(AssetTemplateType type)
        {
            var callLog = Log.Call<string>(type.ToString());
            string result;
            switch (type)
            {
                case AssetTemplateType.Unknown:
                    result = "Unknown file type - cannot provide template";
                    break;
                case AssetTemplateType.CsHtml:
                    result = DefaultCshtmlBody;
                    break;
                case AssetTemplateType.CsHtmlCode:
                    result = DefaultCodeCshtmlBody;
                    break;
                case AssetTemplateType.CsCode:
                    result = DefaultCsCode;
                    break;
                case AssetTemplateType.WebApi:
                    result = DefaultWebApiBody;
                    break;
                case AssetTemplateType.Token:
                    result = DefaultTokenHtmlBody;
                    break;
                case AssetTemplateType.CustomSearchCsCode:
                    result = CustomsSearchCsCode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return callLog(null, result);
        }

        internal string DefaultTokenHtmlBody { get; } = @"<p>
    You successfully created your own template.
    Start editing it by hovering the ""Manage"" button and opening the ""Edit Template"" dialog.
</p>";

        internal abstract string DefaultCshtmlBody { get; }

        internal abstract string DefaultCodeCshtmlBody { get; }

        public const string CsApiTemplateControllerName = "PleaseRenameController";

        // copied from the razor tutorial

        internal string DefaultWebApiBody { get; } = @"#if NETCOREAPP // Oqtane
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else // DNN
using System.Web.Http;
using DotNetNuke.Web.Api;
#endif

[AllowAnonymous]			// define that all commands can be accessed without a login
// Inherit from Custom.Hybrid.Api12 to get features like App, Data...
// see https://docs.2sxc.org/web-api/custom/index.html
public class " + CsApiTemplateControllerName + @" : Custom.Hybrid.Api12
{

    [HttpGet]				// [HttpGet] says we're listening to GET requests
    public string Hello()
    {
        return ""Hello from the controller with ValidateAntiForgeryToken in /api"";
    }

    [HttpPost]				// [HttpPost] says we're listening to POST requests
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

        internal abstract string DefaultCsCode { get; }

        internal abstract string CustomsSearchCsCode { get; }
    }
}
