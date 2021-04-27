using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

// TODO: STV
// - Finish implementation as DI solution
// - Change all access to the constants below to use DI and GetTemplate
// - Then make most of the constants internal properties
// - Then Create a DnnAssetTemplates which inherits from this and register that for Dnn
// - Then create a OqtAssetTemplates which inherits from this
//   - Update templates to do that
// - Also Create Cs Code template for Dnn and Oqtane

namespace ToSic.Sxc.Apps.Assets
{
    [PrivateApi]
    public class AssetTemplates: HasLog<IAssetTemplates>, IAssetTemplates
    {
        public AssetTemplates() : base("SxcAss.Templt")
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
                    result = "TODO STV";
                    break;
                case AssetTemplateType.WebApi:
                    result = DefaultWebApiBody;
                    break;
                case AssetTemplateType.Token:
                    result = DefaultTokenHtmlBody;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return callLog(null, result);
        }

        internal const string DefaultTokenHtmlBody = @"<p>
    You successfully created your own template.
    Start editing it by hovering the ""Manage"" button and opening the ""Edit Template"" dialog.
</p>";

        public const string DefaultCshtmlBody = @"@inherits ToSic.Sxc.Dnn.RazorComponent

<div @Edit.TagToolbar(Content)>
    Put your content here
</div>";

        public const string DefaultCodeCshtmlBody = @"@inherits ToSic.Sxc.Dnn.RazorComponentCode

@functions {
  public string Hello() {
    return ""Hello from inner code"";
  }
}

@helper ShowDiv(string message) {
  <div>@message</div>
}
";
        // probably leave this public / const
        public const string CsApiTemplateControllerName = "PleaseRenameController";

        // copied from the razor tutorial

        public const string DefaultWebApiBody = @"using System.Web.Http;		// this enables [HttpGet] and [AllowAnonymous]
using DotNetNuke.Web.Api;	// this is to verify the AntiForgeryToken

[AllowAnonymous]			// define that all commands can be accessed without a login
[ValidateAntiForgeryToken]	// protects the API from users not on your site (CSRF protection)
// Inherit from ToSic.Custom.Api12 to get features like App, Data...
// see https://docs.2sxc.org/web-api/custom/index.html
public class " + CsApiTemplateControllerName + @" : ToSic.Sxc.Dnn.ApiController
{

    [HttpGet]				// [HttpGet] says we're listening to GET requests
    public string Hello()
    {
        return ""Hello from the controller with ValidateAntiForgeryToken in /api"";
    }

}
";
    }
}
