using ToSic.Razor.Blade;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services.PageShield;

[PrivateApi]
public interface IPageShield
{
    /// <summary>
    /// Add a url-parameter to the allowed list, optionally with specific values which are allowed.
    /// </summary>
    /// <remarks>
    /// * If values are not specified, any value is allowed for the parameter.
    /// * This is used to prevent bots from accessing the page with invalid parameters, which could lead to performance issues or security vulnerabilities.
    /// </remarks>
    /// <param name="keys"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public string Allow(string keys, string? values = null);

    //string? ParametersAllowed { get; }
    //IParameters ParametersUnexpected { get; }
    IParameters Parameters { get; }

    //bool ParametersAreValid { get; }
    [Obsolete("will be removed in v21.07")]
    IHtmlTag? Enforce(ILinkService link, string? prioritize = null);

    /// <summary>
    /// WIP - load a configuration text similar to LightSpeed, ATM just used for the tutorial, not sure if this should be public.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    [PrivateApi]
    string LoadConfiguration(string configuration);

    /// <summary>
    /// Enforce Page Shield - especially Flood Gates - by checking the parameters and returning a tag to be rendered if the parameters are not valid.
    /// This is used to prevent bots from accessing the page with invalid parameters, which could lead to performance issues or security vulnerabilities.
    /// </summary>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="prioritizeParameters">
    /// Specify the url parameters to prioritize in the order of the final url, comma separated.
    /// Optional. All other allowed params will be sorted A-Z.
    /// </param>
    /// <returns></returns>
    IHtmlTag? Enforce(NoParamOrder npo = default, string? prioritizeParameters = null);
}
