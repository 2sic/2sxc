using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.Oqt.Server.Run
{
    [PrivateApi]
    public class OwtAssetTemplates : AssetTemplates
    {
        internal override string DefaultCshtmlBody { get; } = @"@inherits Custom.Hybrid.Razor12

<div @Edit.TagToolbar(Content)>
    Put your content here
</div>";

        internal override string DefaultCodeCshtmlBody { get; } = @"@inherits Custom.Hybrid.Razor12

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

        internal override string DefaultWebApiBody { get; } = @"using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]			// define that all commands can be accessed without a login
[ValidateAntiForgeryToken]	// protects the API from users not on your site (CSRF protection)
// Inherit from Custom.Hybrid.Api12 to get features like App, Data...
// see https://docs.2sxc.org/web-api/custom/index.html
public class " + CsApiTemplateControllerName + @" : Custom.Hybrid.Api12
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
// - This inherits from Custom.Hybrid.Code12
//   which will automatically provide the common objects like App, Dnn etc.
//   from the current context to use in your code

public class FunctionsBasic: Custom.Hybrid.Code12 {
  public string SayHello() {
    return ""Hello from shared functions!"";
    }
}
";

        internal override string CustomsSearchCsCode => "Custom search code not implemented in Oqtane, as Oqtane doesn't have a search index.";
    }
}
