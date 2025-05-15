using ToSic.Eav;

namespace ToSic.Sxc.Apps.Internal.Assets;

public partial class AssetTemplates
{
    // Place each definition in an own static variable.
    // Place the keys, etc. here and use these objects for the constant (don't create separate template-key object etc).
    public static readonly TemplateInfo RazorTyped = new("cshtml-typed", "Razor Typed", ".cshtml", "DetailsTemplate", ForTemplate, TypeRazor)
    {
        Body = @"@inherits Custom.Hybrid.RazorTyped
@* FYI: this inherits from RazorTyped to use the latest APIs. Learn more on https://go.2sxc.org/cs-typed *@

<div @Kit.Toolbar.Default(MyItem)>
    Put your content here
</div>",
        Description = "razor page hybrid typed template",
        
    };

    
    public static readonly TemplateInfo CsTyped =
        new("cs-code-typed", "C# Code Typed", ".cs", "Helpers", ForCode, TypeNone)
        {
            Body = @"// Important notes:
// - This class should have the same name as the file it's in
// - This inherits from Custom.Hybrid.CodeTyped
//   which will automatically provide the common objects like App, MyContext, Data etc.
//   from the current context to use in your code
//   Learn more on https://go.2sxc.org/cs-typed
using ToSic.Sxc.Services; // Make it easier to use https://go.2sxc.org/services

public class " + CsCodeTemplateName + @" : Custom.Hybrid.CodeTyped {
  public string SayHello() {
    return ""Hello from shared functions!"";
  }
}
",
            Description = "c# code typed template",
        };



    public static readonly TemplateInfo ApiTyped =
        new("cs-api-typed", "WebApi Typed", ".cs", "My", ForApi, TypeNone)
        {
            Body = @"#if NETCOREAPP // Oqtane
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else // DNN
using System.Web.Http;
using DotNetNuke.Web.Api;
#endif
using ToSic.Sxc.Services; // Make it easier to use https://go.2sxc.org/services

[AllowAnonymous]      // define that all commands can be accessed without a login
// Inherit from Custom.Hybrid.ApiTyped to get features like App, MyContext, Data etc.
// see https://docs.2sxc.org/web-api/custom/index.html
// Learn more on https://go.2sxc.org/cs-typed
public class " + CsApiTemplateControllerName + @" : Custom.Hybrid.ApiTyped
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
            Description = "c# WebApi controller typed template",
            Suffix = Constants.ApiControllerSuffix,
        };

    /*
    public static readonly TemplateInfo DataSourceTyped =
        new("data-source-typed", "DataSource Typed", ".cs", "MyDataSource", ForDataSource, TypeNone)
        {
            Body = @"// Template Dynamic DataSource - learn about this on https://go.2sxc.org/DsCustom
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSource;

// Class name must match file name, and must extend Custom.DataSource.DataSourceTyped
public class " + CsDataSourceName + @" : Custom.DataSource.DataSource16
{
  // Constructor: must forward MyServices to the base class
  public " + CsDataSourceName + @"(MyServices services) : base(services)
  {
    // ProvideOut will be called when the data is requested
    // in this example it will return a list on the Default stream
    ProvideOut(() => {
            // Dummy list of numbers using the Configuration ""AmountOfItems""
            var listOfNumbers = Enumerable.Range(1, AmountOfItems);

                // Create a list of anonymous objects with some data
                return listOfNumbers.Select(i => new {
            Id = i,
            guid = Guid.NewGuid(),
            Title = Greeting
        }).ToList();
    });
}

// Configuration ensures that there is a config with the name ""Greeting""
[Configuration(Fallback = ""Hello from Template DataSource"")]
public string Greeting { get { return Configuration.GetThis(); } }

// Another configuration. Since it's an int,
// we must provide a fallback in the GetThis(3) in case the incoming config is not a number
[Configuration(Fallback = 10)]
public int AmountOfItems { get { return Configuration.GetThis(3); } }
}
",
            Description = "c# DataSource hybrid template",
            Suffix = "",
            Folder = "DataSources",
        };
    */
}