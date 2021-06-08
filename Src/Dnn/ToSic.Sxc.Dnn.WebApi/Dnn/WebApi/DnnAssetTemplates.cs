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

public class " + CsCodeTemplateName + @": ToSic.Sxc.Code.WithDnnContext {
  public string SayHello() {
    return ""Hello from shared functions!"";
    }
}
";

    internal override string CustomsSearchCsCode { get; } = @"using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Context;
using ToSic.Sxc.Search;
/*
Custom code which views use to customize how dnn search treats data of that view.
It's meant for customizing the internal indexer of the platform, _not_ for Google Search.

To use it, create a custom code (.cs) file which implements ICustomizeSearch interface.
You can also inherit from a DynamicCode base class (like Code12) if you need more functionality.

For more guidance on search customizations, see https://r.2sxc.org/customize-search
*/
public class " + CsCodeTemplateName + @" : Custom.Hybrid.Code12, ICustomizeSearch
{
    /// <summary>
    /// Populate the search
    /// </summary>
    /// <param name=""searchInfos"">Dictionary containing the streams and items in the stream for this search.</param>
    /// <param name=""moduleInfo"">Module information with which you can find out what page it's on etc.</param>
    /// <param name=""beginDate"">Last time the indexer ran - because the data you will get is only what was modified since.</param>
    public void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate)
    {
        // Set this to true if you want to see logs of this search in the insights
        // Only do this while developing, otherwise you'll flood the logs and never see the important parts
        Log.Preserve = false;
        
        foreach (var si in searchInfos[""AllPosts""])
        {
            var entity = AsDynamic(si.Entity);
            si.Title = ""Title: "" + entity.Title;
            si.QueryString = ""details="" + entity.UrlKey;
        }

        // Remove not needed streams
        var keys = searchInfos.Keys.ToList();
        foreach (var key in keys)
        {
            if (key != ""AllPosts"")
            {
                searchInfos.Remove(key);
            }
        }
    }
}
";
    }
}
