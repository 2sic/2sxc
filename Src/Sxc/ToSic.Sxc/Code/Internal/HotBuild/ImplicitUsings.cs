namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class ImplicitUsings
{
    /// <summary>
    /// Usings which are implicitly added to all generated code files.
    /// It's important that they are the same in Dnn and Oqtane, and that it's not too much,
    /// because that ensures that the code created by others has to mention their usings explicitly.
    ///
    /// Otherwise, if we once shrink this list, then all code created by others could break.
    /// </summary>
    public static readonly List<string> ForRazor =
    [
        // 1. based on 'obj/*/*.GlobalUsings.g.cs' for Microsoft.NET.Sdk
        "System",
        "System.Collections.Generic",
        //"System.IO", // not implicit using in Oqtane razor
        "System.Linq",
        //"System.Net.Http", // not referenced in DNN razor
        //"System.Threading", // not implicit using in DNN razor
        //"System.Threading.Tasks", // not implicit using in DNN razor

        // 2. based on 'obj/*/*.GlobalUsings.g.cs' for Microsoft.NET.Sdk.Web
        //"System.Net.Http.Json", // not referenced in DNN razor
        //"Microsoft.AspNetCore.Builder", // not referenced in DNN razor
        //"Microsoft.AspNetCore.Hosting", // not referenced in DNN razor
        //"Microsoft.AspNetCore.Http", // not referenced in DNN razor
        //"Microsoft.AspNetCore.Routing", // not referenced in DNN razor
        //"Microsoft.Extensions.Configuration", // not implicit using in DNN razor
        //"Microsoft.Extensions.DependencyInjection", // not implicit using in DNN razor
        //"Microsoft.Extensions.Hosting", // not referenced in DNN razor
        //"Microsoft.Extensions.Logging", // not implicit using in DNN razor

        // 3. other usings
        //"System.Text", // not implicit using in DNN razor
        //"System.Web", // not implicit using in Oqtane razor
        //"System.Web.UI", // not referenced in Oqtane razor
        //"System.Web.UI.WebControls", // not referenced in Oqtane razor
        //"System.Web.WebPages", // not referenced in Oqtane razor
    ];
}