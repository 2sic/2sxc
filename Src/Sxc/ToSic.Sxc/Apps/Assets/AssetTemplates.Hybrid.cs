namespace ToSic.Sxc.Apps.Assets
{
    public partial class AssetTemplates
    {
        // TODO STV
        // Place each definition in an own static variable
        // Place the keys, etc. here and use these objects for the constant (don't create separate template-key object etc.)
        // then _don't_ put this in an abstract class, just in an own class containing all the data
        // ...and put your 
        public static readonly TemplateInfo RazorHybrid = new TemplateInfo("cshtml-hybrid", "Razor Hybrid", ".cshtml", "DetailsTemplate", ForTemplate, TypeRazor)
        {
            Body = @"@inherits Custom.Hybrid.Razor12
@* This inherits statement gets you features like App, CmsContext, Data etc. - you can delete this comment *@

<div @Edit.TagToolbar(Content)>
    Put your content here
</div>",
            Description = "razor page hybrid template",
            Prefix = "_",
        };

        public static readonly TemplateInfo CsHybrid =
            new TemplateInfo("cs-code-hybrid", "C# Code Hybrid", ".cs", "Helpers", ForCode, TypeNone)
            {
                Body = @"// Important notes:
// - This class should have the same name as the file it's in
// - This inherits from Custom.Hybrid.Code12
//   which will automatically provide the common objects like App, CmsContext, Data etc.
//   from the current context to use in your code

public class " + CsCodeTemplateName + @" : Custom.Hybrid.Code12 {
  public string SayHello() {
    return ""Hello from shared functions!"";
  }
}
",
                Description = "c# code hybrid template",
            };



        public static readonly TemplateInfo ApiHybrid =
            new TemplateInfo("cs-api-hybrid", "WebApi Hybrid", ".cs", "My", ForApi, TypeNone)
            {
                Body = @"#if NETCOREAPP // Oqtane
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
",
                Description = "c# WebApi controller hybrid template",
                Suffix = "Controller",
            };

    }
}
