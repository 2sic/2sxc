using ToSic.Razor.Markup;

namespace ToSic.Sxc.Services;

public partial interface IPageService
{
    #region Security

    /// <summary>
    /// Add common html attributes to a `script` or `link` tag to [enable optimizations](xref:Basics.Server.AssetOptimization.Index)
    /// and [automatically whitelist in the Content Security Policy](xref:Abyss.Security.Csp.Parts#auto-white-listing-explicit)
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="optimize">Activate optimize, default is true</param>
    /// <param name="priority">Optional priority of optimization. Must be more than 100 to have an effect.</param>
    /// <param name="position">Optional position of the resource (`head`, `body`, `bottom`)</param>
    /// <param name="whitelist">Automatically add to CSP-whitelist. This uses a random key to protect against XSS.</param>
    /// <returns>The asset attributes in a format which will be preserved in HTML</returns>
    /// <remarks>
    /// History: Created in 2sxc 13.10
    /// </remarks>
    IRawHtmlString AssetAttributes(
        NoParamOrder noParamOrder = default,
        bool optimize = true,
        int priority = 0,
        string position = null,
        bool whitelist = true);

    /// <summary>
    /// Add a CSP rule where you also specify the name.
    /// Best check the [CSP Guide](xref:Abyss.Security.Csp.Index).
    ///
    /// For an example, see [Coded CSP](xref:Abyss.Security.Csp.CodedRules)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="values"></param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddCsp(...)`</returns>
    string AddCsp(string name, params string[] values);

    #endregion
}