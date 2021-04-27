using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.Dnn.WebApi
{
    public class DnnAssetTemplates : AssetTemplates
    {
        internal override string DefaultCshtmlBody { get; } = @"@inherits ToSic.Sxc.Dnn.RazorComponent

<div @Edit.TagToolbar(Content)>
    Put your content here
</div>";

        internal override string DefaultCodeCshtmlBody { get; } = @"@inherits ToSic.Sxc.Dnn.RazorComponentCode

@functions {
  public string Hello() {
    return ""Hello from inner code"";
  }
}

@helper ShowDiv(string message) {
  <div>@message</div>
}
";

        // copied from the razor tutorial

        internal override string DefaultWebApiBody { get; } = @"using System.Web.Http;		// this enables [HttpGet] and [AllowAnonymous]
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

        internal override string DefaultCsCode { get; } = @"// Important notes:
// - This class should have the same name as the file it's in
// - This inherits from ToSic.Sxc.Code.WithDnnContext
//   which will automatically provide the common objects like App, Dnn etc.
//   from the current context to use in your code

public class FunctionsBasic: ToSic.Sxc.Code.WithDnnContext {
  public string SayHello() {
    return ""Hello from shared functions!"";
    }
}
";
    }
}
